using ApplicationServices.DTOs;
using DomainCore.Entities;
using DomainCore.Interfaces;
using AutoMapper;
using System.Security.Cryptography;
namespace ApplicationServices.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public DocumentService(IDocumentRepository documentRepository, IMapper mapper)
        {
            _documentRepository = documentRepository;
            _mapper = mapper;
        }

        public async Task<DocumentDTO> Get(string token)
        {
            // Validate the access token and retrieve document ID from it
            int documentId = _tokenService.ValidateDocumentAccessToken(token).Value;

            if (documentId == null)
                throw new UnauthorizedAccessException("Invalid access token.");

            var document = await _documentRepository.Get(documentId);
            var documentDto = _mapper.Map<DocumentDTO>(document);
            documentDto.Url = token;
            return documentDto;
        }
        public async Task<DocumentDownloadDTO> GetSavedName(string token)
        {
            // Validate the access token and retrieve document ID from it
            int documentId = _tokenService.ValidateDocumentAccessToken(token).Value;

            if (documentId == null)
                throw new UnauthorizedAccessException("Invalid access token.");

            var document = await _documentRepository.Get(documentId);
            var documentDto = _mapper.Map<DocumentDownloadDTO>(document);
            return documentDto;
        }
        public async Task<Document> Add(DocumentCreateDTO DocumentDTO)
        {
            var document = _mapper.Map<Document>(DocumentDTO);
            // Generate hash for the document content here
            var addedDoc = await _documentRepository.Add(document);
            document.Url = _tokenService.GenerateDocumentAccessToken(addedDoc.Id, addedDoc.UserId);
            var updatedDoc = await _documentRepository.Update(addedDoc);
            return updatedDoc;
        }

        public async Task<IEnumerable<DocumentDTO>> GetAll(int userId)
        {
            var documents = await _documentRepository.GetAll(userId);
            if (documents == null || !documents.Any())
            {
                // Log the situation, return an empty list, or throw an appropriate exception
                return new List<DocumentDTO>();
            }

            return _mapper.Map<IEnumerable<DocumentDTO>>(documents);

        }

        public async Task Delete(int id)
        {
            await _documentRepository.Delete(id);
        }

        public Task<DocumentDTO> Update(string token, DocumentDTO doc_dto)
        {
            throw new NotImplementedException();
        }

        public async Task Delete(string token)
        {
            int documentId = _tokenService.ValidateDocumentAccessToken(token).Value;
            await _documentRepository.Delete(documentId);

        }

        public Task<string> ValidateHash(DocumentDownloadDTO doc_dto)
        {

            throw new NotImplementedException();
        }


        public string HashDocumentContent(Stream documentStream)
        {
            // Ensure the stream is at the start
            documentStream.Position = 0;

            using (var sha256 = SHA256.Create())
            {
                // Compute the hash from the document stream
                var hashBytes = sha256.ComputeHash(documentStream);

                // Convert the byte array to a readable hex string
                var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

                return hashString;
            }
        }

        Task<string> IDocumentService.HashDocumentContent(Stream documentStream)
        {
            throw new NotImplementedException();
        }

        public string GenerateAccessToken(int documentId, int userId)
        {
            var token = _tokenService.GenerateDocumentAccessToken(documentId, userId);
            return token;
        }

        public async Task<bool> ValidateHash(Stream stream, string hash)
        {
            string fileHash = HashDocumentContent(stream);
            if (fileHash == hash)
            {
                return true;
            }
            return false;
        }


    }

}