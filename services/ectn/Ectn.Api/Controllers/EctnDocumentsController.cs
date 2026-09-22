using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Ectn.Api.Controllers;

[ApiController]
[Route("api/v1/ectn/{ectnId}/documents")]
[Authorize]
public class EctnDocumentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EctnDocumentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = "ectn.documents.upload")]
    [RequestSizeLimit(10485760)] // 10 MB limit for security (Rule 48/49)
    public async Task<IActionResult> UploadDocument(Guid ectnId, IFormFile file, [FromForm] string documentType)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is empty or missing.");

        // Additional MIME and Extension Validation logic would live here
        
        // Pseudo-command: UploadDocumentCommand
        return Accepted(new { Message = "Document uploaded and pending malware scan." });
    }

    [HttpGet("{documentId}")]
    [Authorize(Policy = "ectn.documents.read")]
    public async Task<IActionResult> DownloadDocument(Guid ectnId, Guid documentId)
    {
        // Enforce tenant isolation by verifying EctnId -> TenantId
        return Ok(new { Url = "https://minio.local/signed-url-for-download" });
    }
}
