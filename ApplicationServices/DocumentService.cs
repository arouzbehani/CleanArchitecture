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
            var documentDto=_mapper.Map<DocumentDTO>(document);
            documentDto.Url=token;
            return documentDto;
        }

        public async Task<DocumentDTO> Add(DocumentDTO DocumentDTO)
        {
            var document = _mapper.Map<Document>(DocumentDTO);
            // Generate hash for the document content here
            document.Hash = GenerateHash(DocumentDTO);
            var addedDoc = await _documentRepository.Add(document);
            return _mapper.Map<DocumentDTO>(addedDoc);
        }

        public async Task<IEnumerable<DocumentDTO>> GetAll()
        {
            var documents = await _documentRepository.GetAll();
            return _mapper.Map<IEnumerable<DocumentDTO>>(documents);
        }

        public async Task Delete(int id)
        {
            await _documentRepository.Delete(id);
        }


        private string GenerateHash(DocumentDTO DocumentDTO)
        {
            // Your hash generation logic (e.g., SHA256) for document content
            return "someHashValue";
        }

        public Task<DocumentDTO> Update(string token, DocumentDTO doc_dto)
        {
            throw new NotImplementedException();
        }

        public Task Delete(string token)
        {
            throw new NotImplementedException();
        }

        public Task<string> ValidateHash(DocumentDTO doc_dto, string hash)
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
    }

}