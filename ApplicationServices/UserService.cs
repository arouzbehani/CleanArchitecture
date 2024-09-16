using ApplicationServices.DTOs;
using DomainCore.Entities;
using DomainCore.Interfaces;
using AutoMapper;
using System.Security.Cryptography;
using System.Security.Claims;
namespace ApplicationServices.Services
{
    public class UserService : IUserService
    {
        private readonly ISecurityService _securityService;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public UserService(IUserRepository userRepository, IMapper mapper, ISecurityService securityService, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _securityService = securityService;
            _tokenService = tokenService;
            
        }

        public async Task<UserDTO> AddUser(UserCreationDTO userDto)
        {
            string salt;
            var hashedPassword = _securityService.HashPassword(userDto.Password, out salt);
            var user = _mapper.Map<User>(userDto);
            user.HashedPassword = hashedPassword;
            user.Salt = salt;

            var addedUser = await _userRepository.AddUser(user);
            return _mapper.Map<UserDTO>(addedUser);
        }
        public async Task<string> ValidateUser(UserLoginDTO userLoginDto)
        {
            var user = await _userRepository.GetUserByEmail(userLoginDto.Email);
            if (user == null || userLoginDto.Password != userLoginDto.Password) // Simplified; use hashed passwords in production
            {
                return null;
            }
            var token = _tokenService.GenerateAuthenticationToken(user.Id.ToString(), user.Email);

            return token;
        }
        public async Task<UserDTO> GetUser(string token)
        {
            var userId=_tokenService.GetUserIdByToken(token);
            var user = await _userRepository.GetUser(userId);
            if (user == null) return null;

            return _mapper.Map<UserDTO>(user);

        }
        public async Task<User> GetUserWithId(string token)
        {
            var userId=_tokenService.GetUserIdByToken(token);
            var user = await _userRepository.GetUser(userId);
            if (user == null) return null;

            return user;

        }
        public async Task DeleteUser(string token)
        {
            int id=_tokenService.GetUserIdByToken(token);
            await _userRepository.DeleteUser(id);
        }
        public async Task<IEnumerable<UserDTO>> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsers();
            return users.Select(user => _mapper.Map<UserDTO>(user)).ToList();
        }


        public async Task<UserDTO> UpdateUser(string token, UserUpdateDTO userUpdateDto)
        {
            var userId=_tokenService.GetUserIdByToken(token);
            var existingUser = await _userRepository.GetUser(userId);
            if (existingUser == null)
            {
                throw new Exception("User not found");
            }
            var hashedPassword = existingUser.HashedPassword;
            var salt = existingUser.Salt;
            _mapper.Map(userUpdateDto, existingUser);
            existingUser.HashedPassword = hashedPassword;
            existingUser.Salt = salt;

            var updatedUser = await _userRepository.UpdateUser(existingUser);
            return _mapper.Map<UserDTO>(updatedUser);
        }

        public string RetrieveToken(string authHeader)
        {
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return "";
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            return token;
        }



        // Other business logic methods...
    }
}
