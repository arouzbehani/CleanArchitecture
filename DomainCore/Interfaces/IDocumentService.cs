using DomainCore.Entities;
using ApplicationServices.DTOs;
    public interface IDocumentService
    {
        Task<Document> Add(DocumentCreateDTO doc_dto);
        Task<DocumentDTO> Get(string token);
        Task<DocumentDownloadDTO> GetSavedName(string token);
        Task<IEnumerable<DocumentDTO>> GetAll(int userId);  // Updated to return Task<IEnumerable<>>
        Task<DocumentDTO> Update(string token,DocumentDTO doc_dto);
        Task Delete(string token);  // Updated to return Task
        Task<bool> ValidateHash(Stream stream, string hash); // if successful --> returns jwt token
        Task<string> HashDocumentContent(Stream documentStream);
        string GenerateAccessToken(int documentId,int userId);

    }