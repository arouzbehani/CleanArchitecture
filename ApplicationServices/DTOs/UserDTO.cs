// File: ApplicationServices/DTOs/UserDTO.cs
using DomainCore.Entities;

namespace ApplicationServices.DTOs
{
    public class UserDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? ProfilePicUrl { get; set; }
        public string? Phone { get; set; }

    }
    public class UserUpdateDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ProfilePicUrl { get; set; }
        public string? Phone { get; set; }

    }    
    public class UserCreationDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string Password{get;set;}
    }

    public class UserLoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    
}
