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

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly string _jwtSecret;

        public UserController(UserService userService,IConfiguration config)
        {
            _userService = userService;
            _jwtSecret = config["Jwt:Secret"];  // Ensure this is set in your configuration

        }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDTO loginDto)
    {
        var userDto =await _userService.ValidateUser(loginDto);
        if (userDto == null)
            return Unauthorized("Invalid credentials");
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var key=new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(key);
        }
        var securityKey = new SymmetricSecurityKey(key);

        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString()),
            new Claim(ClaimTypes.Email, userDto.Email)
            // Other claims can be added here
        };       
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(24*7),
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return  Ok(new { Token = tokenString });
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

        // Other actions...
    }
}
