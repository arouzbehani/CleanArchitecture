using ApplicationServices.DTOs;
using ApplicationServices.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using AutoMapper;
using DomainCore.Entities;
using DomainCore.Interfaces;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public DocumentController(IDocumentService documentService, ITokenService tokenService, IMapper mapper)
        {
            _documentService = documentService;
            _tokenService = tokenService;
            _mapper = mapper;

        }
        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddDocument([FromForm] DocumentDTO documentDto, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                // Hash the document content
                using var fileReadStream = file.OpenReadStream();
                string documentHash = _documentService.HashDocumentContent(fileReadStream);


                var folderName = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "documents");
                if (!Directory.Exists(folderName))
                {
                    Directory.CreateDirectory(folderName);
                }
                var savedName = Path.GetRandomFileName();
                var filePath = Path.Combine(folderName, savedName);
                // Save the document to the specified folder
                using (var fileCreateStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileCreateStream);
                }
                // Save document metadata and hash to the database
                var newDocument = new Document
                {
                    Name = documentDto.Name,
                    Description = documentDto.Description,
                    FileType = file.ContentType,
                    Size = file.Length,
                    Hash = documentHash,
                    DateUploaded = DateTime.UtcNow,
                    SavedName = savedName
                };

                await _documentService.Add(_mapper.Map<DocumentDTO>(newDocument));
                return Ok(new { message = "Document added successfully", documentId = newDocument.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error while uploading document: " + ex.Message);
            }
        }
        [HttpGet("view")]
        public async Task<IActionResult> View([FromQuery] string accessToken)
        {

            var documentDto = await _documentService.Get(accessToken);
            return Ok(documentDto);
        }


        [HttpDelete("{token}")]
        public async Task<IActionResult> Delete(string token)
        {
            await _documentService.Delete(token);
            return NoContent();
        }
        // Other document-related endpoints
        [HttpGet("documents/{accessToken}")]
        public async Task<IActionResult> DownloadDocument(string token)
        {
            try
            {
                // Retrieve the document by its ID (you can use your repository/service here)
                var documentDto = await _documentService.Get(token);

                if (documentDto == null)
                {
                    return NotFound("Document not found");
                }

                var documentPath = Path.Combine("Documents", documentDto.SavedName);

                if (!System.IO.File.Exists(documentPath))
                {
                    return NotFound("Document file not found on the server");
                }

                // Serve the file for download
                var memoryStream = new MemoryStream();
                using (var stream = new FileStream(documentPath, FileMode.Open))
                {
                    await stream.CopyToAsync(memoryStream);
                }

                memoryStream.Position = 0;
                return File(memoryStream, "application/octet-stream", documentDto.SavedName);
            }
            catch (Exception)
            {
                return Unauthorized("Invalid access token");
            }
        }

    }

}
