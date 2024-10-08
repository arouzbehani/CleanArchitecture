using ApplicationServices.DTOs;
using DomainCore.Entities;
    public interface IUserService
    {
        Task<UserDTO> AddUser(UserCreationDTO userDto);
        Task<UserDTO> GetUser(string token);
        Task<User> GetUserWithId(string token);
        Task<IEnumerable<UserDTO>> GetAllUsers();  // Updated to return Task<IEnumerable<>>
        Task<UserDTO> UpdateUser(string token,UserUpdateDTO userUpdateDto);
        Task DeleteUser(string token);  // Updated to return Task
        Task<string> ValidateUser(UserLoginDTO userLoginDTO); // if successful --> returns jwt token
        string RetrieveToken(string authHeader);
        Task<bool> UserIsValid(string token);
     
    }