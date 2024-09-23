namespace ApplicationServices.DTOs
{

    public class DocumentDTO
    {
        public string DocumentName { get; set; }
        public string? Description { get; set; }
        public string FileType { get; set; }
        public long Size { get; set; }
        public DateTime DateUploaded { get; set; }
        public string Url { get; set; }
    }
    public class DocumentCreateDTO
    {
        public string DocumentName { get; set; }
        public string? Description { get; set; }
        public string FileType { get; set; }
        public long Size { get; set; }
        public DateTime DateUploaded { get; set; }
        public string Hash { get; set; }
        public int UserId { get; set; }
        public string SavedName { get; set; }   
    }    
    public class DocumentDownloadDTO
    {
        public string Hash { get; set; }
        public string SavedName { get; set; }   
    }    
}