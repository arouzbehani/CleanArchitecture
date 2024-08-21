using DomainCore.Entities;

namespace DomainCore.Interfaces
{
    public interface IUserRepository
    {
        Task<User>  AddUser(User user);
        Task<User>  GetUser(int id);
        Task<User>  GetUserByEmail(string email);

        // Other methods like Update, Delete, etc.
        Task<IEnumerable<User>> GetAllUsers();
        Task<User> UpdateUser(User user);
        Task DeleteUser(int id);
    }
}
