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
        private readonly IUserService _userService;
        private readonly IDocumentService _documentService;
        private readonly IMapper _mapper;

        public DocumentController(IDocumentService documentService, IUserService userService, IMapper mapper)
        {
            _documentService = documentService;
            _userService = userService;
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
                var authHeader = Request.Headers["Authorization"].ToString();
                var token = _userService.RetrieveToken(authHeader);
                var userDto = await _userService.GetUser(token);
                if (userDto == null)
                {
                    return NotFound("Not Authorized User Access Denied!");
                }
                // Hash the document content
                using var fileReadStream = file.OpenReadStream();
                var hashed=await _documentService.HashDocumentContent(fileReadStream);

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
                int userId=_userService.GetUserWithId(token).Id;
                var newDocument = new DocumentCreateDTO
                {
                    Name = documentDto.Name,
                    Description = documentDto.Description,
                    FileType = file.ContentType,
                    Size = file.Length,
                    DateUploaded = DateTime.UtcNow,
                    Hash=hashed,
                    UserId=userId
                };

                Document addedDocument=await _documentService.Add(newDocument);
                return Ok(new { message = "Document added successfully", document=_mapper.Map<DocumentDTO>(addedDocument) });
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
        [HttpGet("gallery/{accessToken}")]
        public async Task<IActionResult> GalleryView([FromQuery] string accessToken)
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
        [HttpGet("download/{token}")]
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
