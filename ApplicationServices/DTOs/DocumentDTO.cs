namespace ApplicationServices.DTOs
{

    public class DocumentDTO
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string FileType { get; set; }
        public long Size { get; set; }
        public DateTime DateUploaded { get; set; }
        public string Url { get; set; }
        public string SavedName{get;set;}
    }

}