using ApplicationServices.DTOs;
using ApplicationServices.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(UserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO loginDto)
        {
            var token = await _userService.ValidateUser(loginDto);
            if (token == null)
                return Unauthorized("Invalid credentials");

            try
            {
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                _logger.LogError(ex, "Error generating token");
                return StatusCode(500, "An error occurred while generating the token.");
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> AddUser([FromBody] UserCreationDTO userCreationDto)
        {
            if (userCreationDto == null)
            {
                return BadRequest("User cannot be null.");
            }

            var userDto = await _userService.AddUser(userCreationDto);

            // Return the created user with status 201
            return Ok(userDto);
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDTO>> GetUser()
        {
            var authHeader = Request.Headers["Authorization"].ToString();

            var token = RetrieveToken(authHeader);
            var userDto = await _userService.GetUser(token);

            if (userDto == null)
            {
                return NotFound();
            }

            return Ok(userDto);
        }
        [HttpPut("update")]
        [Authorize]
        public async Task<ActionResult<UserUpdateDTO>> UpdateUser([FromBody] UserUpdateDTO userUpdateDto)
        {
            // Extract the token from the Authorization header

            var authHeader = Request.Headers["Authorization"].ToString();

            var token = RetrieveToken(authHeader);
            var user = await _userService.GetUser(token);

            if (user == null)
            {
                return NotFound("User not found.");
            }
            var updateUser = await _userService.UpdateUser(token, userUpdateDto);

            return Ok(updateUser);
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> GetProfile()
        {
            // Extract the token from the Authorization header
            var authHeader = Request.Headers["Authorization"].ToString();

            var token = RetrieveToken(authHeader);

            // Retrieve the user data using the userId
            UserDTO userDto = await _userService.GetUser(token);
            if (userDto == null)
            {
                return NotFound("User not found.");
            }

            return Ok(userDto);
        }
        private string RetrieveToken(string authHeader)
        {
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return "";
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            return token;
        }

    }
}
