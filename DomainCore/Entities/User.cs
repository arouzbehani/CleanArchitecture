namespace DomainCore.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string LastName { get; set; }
        public string? Phone { get; set; }
        public string Email { get; set; }
        public string HashedPassword {get;set;}
        public string Salt {get;set;}
        public string? ProfilePicUrl {get;set;}
        public ICollection<Document> Documents{get;set;}

        // Other properties...
    }
}
