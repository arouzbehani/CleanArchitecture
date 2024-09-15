namespace DomainCore.Entities
{
    public class Document
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string FileType { get; set; }
        public long Size { get; set; }
        public DateTime DateUploaded { get; set; }
        public string Hash {get;set;}
        public string SavedName{get;set;}
    }
}