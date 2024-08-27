// File: ApplicationServices/DTOs/UserDTO.cs
using DomainCore.Entities;

namespace ApplicationServices.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

    }
    public class UserUpdateDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ProfilePicUrl { get; set; }

    }    
    public class UserCreationDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password{get;set;}

    }

    public class UserLoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    
}
