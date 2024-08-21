using ApplicationServices.DTOs;
    public interface IUserService
    {
        Task<UserDTO> AddUser(UserCreationDTO userDto);
        Task<UserDTO> GetUser(int id);
        Task<IEnumerable<UserDTO>> GetAllUsers();  // Updated to return Task<IEnumerable<>>
        Task<UserDTO> UpdateUser(UserDTO userDto);
        Task DeleteUser(int id);  // Updated to return Task
        Task<UserDTO> ValidateUser(UserLoginDTO userLoginDTO);

    }