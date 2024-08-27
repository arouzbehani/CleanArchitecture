namespace DomainCore.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password {get;set;}
        public string HashedPassword {get;set;}
        public string Salt {get;set;}
        public string ProfilePicUrl {get;set;}

        // Other properties...
    }
}
