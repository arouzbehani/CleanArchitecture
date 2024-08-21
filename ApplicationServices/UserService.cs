using ApplicationServices.DTOs;
using DomainCore.Entities;
using DomainCore.Interfaces;
using AutoMapper;
namespace ApplicationServices.Services
{
    public class UserService:IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDTO> AddUser(UserCreationDTO userDto)
        {
            var user = _mapper.Map<User>(userDto);
            var addedUser = await _userRepository.AddUser(user);
            return _mapper.Map<UserDTO>(addedUser);
        }

        public async Task DeleteUser(int id)
        {
            await _userRepository.DeleteUser(id);
        }
        public async Task<IEnumerable<UserDTO>> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsers();
            return users.Select(user => _mapper.Map<UserDTO>(user)).ToList();
        }

        public async Task<UserDTO> GetUser(int id)
        {
            var user = await _userRepository.GetUser(id);
            if (user == null) return null;

            return _mapper.Map<UserDTO>(user);

        }

        public async Task<UserDTO> UpdateUser(UserDTO userDto)
        {
            var user = _mapper.Map<User>(userDto);
            var updatedUser = await _userRepository.UpdateUser(user);
            return _mapper.Map<UserDTO>(updatedUser);
        }
        public async Task<UserDTO> ValidateUser(UserLoginDTO userLoginDto)
        {
            var user = await _userRepository.GetUserByEmail(userLoginDto.Email);
            if (user == null || userLoginDto.Password != userLoginDto.Password) // Simplified; use hashed passwords in production
            {
                return null;
            }

            return _mapper.Map<UserDTO>(user);
        }

        // Other business logic methods...
    }
}
