using ApplicationServices.DTOs;
using ApplicationServices.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using AutoMapper;
using DomainCore.Entities;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;

        public UserController(UserService userService, ILogger<UserController> logger,IMapper mapper)
        {
            _userService = userService;
            _logger = logger;
            _mapper=mapper;
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
        public async Task<ActionResult<UserUpdateDTO>> UpdateUser([FromForm] UserUpdateDTO userUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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
        [HttpPut("updateImage")]
        [Authorize]
        public async Task<ActionResult<UserUpdateDTO>> UpdateUserImage([FromForm] IFormFile? profilePic)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            

            // Extract the token from the Authorization header
            var authHeader = Request.Headers["Authorization"].ToString();
            var token = RetrieveToken(authHeader);
            var user = await _userService.GetUser(token);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (profilePic != null && profilePic.Length > 0)
            {
                // Save the file to the server (optional: validate file type, size, etc.)
                var folderName = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profile-pics");

                // Ensure the directory exists
                if (!Directory.Exists(folderName))
                {
                    Directory.CreateDirectory(folderName);
                }

                // Generate a unique filename
                var fileName = profilePic.FileName;
                // var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                var fileNameWithoutExtension = "Profile pic";
                var extension = Path.GetExtension(fileName);

                // Ensure unique filename if file exists
                Random random = new Random();
                int count = random.Next(1000, 9999);
                string newFileName;
                string newFullPath;

                do
                {
                    newFileName = $"{fileNameWithoutExtension}_{count++}{extension}";
                    newFullPath = Path.Combine(folderName, newFileName);
                } while (System.IO.File.Exists(newFullPath));

                // Save the file
                using (var stream = new FileStream(newFullPath, FileMode.Create))
                {
                    await profilePic.CopyToAsync(stream);
                }

                // Update ProfilePicUrl in UserUpdateDTO
                var userUpdateDto=_mapper.Map<UserUpdateDTO>(user);
                userUpdateDto.ProfilePicUrl = $"/profile-pics/{Path.GetFileName(newFullPath)}";

                // Delete the old file if it exists
                if (user.ProfilePicUrl != null)
                {
                    var oldFilePath = Path.Combine(folderName, Path.GetFileName(user.ProfilePicUrl)); // Assuming user.ProfilePicUrl has the old file path
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
            var updateUser = await _userService.UpdateUser(token, userUpdateDto);
            return Ok(updateUser);

            }
            return BadRequest(ModelState);

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
