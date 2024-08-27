using ApplicationServices.DTOs;
using ApplicationServices.Services;
using DomainCore.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly TokenService _tokenService;
        private readonly ILogger<UserController> _logger;
    private readonly JwtSettings _jwtSettings;

        public UserController(UserService userService,TokenService tokenService,ILogger<UserController> logger)
        {
            _userService = userService;
            _tokenService=tokenService;
            _logger=logger;
        }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDTO loginDto)
    {
        var userDto =await _userService.ValidateUser(loginDto);
        if (userDto == null)
            return Unauthorized("Invalid credentials");

        try
        {
            var token = _tokenService.GenerateToken(userDto.Id.ToString(), userDto.Email);
            return Ok(new { Token = token });
        }
        catch (Exception ex)
        {
            // Log the exception for debugging
            _logger.LogError(ex, "Error generating token");
            return StatusCode(500, "An error occurred while generating the token.");
        }
    }
  
        [HttpPost]
        public async Task<ActionResult<UserDTO>> AddUser([FromBody] UserCreationDTO userCreationDto)
        {
            if (userCreationDto == null)
            {
                return BadRequest("User cannot be null.");
            }

            var userDto = await _userService.AddUser(userCreationDto);

            // Return the created user with status 201
            return CreatedAtAction(nameof(GetUser), new { id = userDto.Id }, userDto);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var user = await _userService.GetUser(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
        [HttpPut("update")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> UpdateUser([FromBody] UserDTO userDto)
        {
            // Extract the token from the Authorization header
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized("No token provided.");
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            
            var userId = ReturnUserId(token);
            userDto.Id=userId;            
            var user = await _userService.GetUser(userDto.Id);

            if (user == null)
            {
                return NotFound("User not found.");
            }
            var updateUser=await _userService.UpdateUser(userDto);

            return Ok(updateUser);
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            // Extract the token from the Authorization header
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized("No token provided.");
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            
            var userId = ReturnUserId(token);

            // Retrieve the user data using the userId
            var userDto = await _userService.GetUser(userId);
            if (userDto == null)
            {
                return NotFound("User not found.");
            }

            return Ok(userDto);
        }
        public int ReturnUserId(string token){

            // Decode the token and get the user claims
            var principal = _tokenService.GetPrincipalFromToken(token);
            if (principal == null)
            {
                return 0;
            }

            // Extract the userId claim
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return 0;
            }

            var userId = int.Parse(userIdClaim.Value);
            return userId;
        }
    }
}
