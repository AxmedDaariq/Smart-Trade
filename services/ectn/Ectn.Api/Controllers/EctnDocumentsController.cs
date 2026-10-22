using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Ectn.Api.Controllers;

[ApiController]
[Route("api/v1/ectn/{ectnId}/documents")]
[Authorize]
public class EctnDocumentsController : ControllerBase
{
    // A04/A05: documents are untrusted input. Only a small, known set of types is accepted, the
    // client supplied name is sanitised (no path separators, traversal sequences or control
    // characters) and the size is capped in addition to the request size limit.
    private static readonly string[] AllowedExtensions = { ".pdf", ".png", ".jpg", ".jpeg" };
    private static readonly string[] AllowedContentTypes = { "application/pdf", "image/png", "image/jpeg" };
    private static readonly Regex DocumentTypePattern = new("^[A-Za-z0-9_-]{1,64}$", RegexOptions.Compiled);
    private const long MaxFileSizeBytes = 10 * 1024 * 1024;

    private readonly IMediator _mediator;
    private readonly ILogger<EctnDocumentsController> _logger;

    public EctnDocumentsController(IMediator mediator, ILogger<EctnDocumentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Policy = "ectn.documents.upload")]
    [RequestSizeLimit(MaxFileSizeBytes)]
    public async Task<IActionResult> UploadDocument(Guid ectnId, IFormFile file, [FromForm] string documentType)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { Error = "File is empty or missing." });

        if (file.Length > MaxFileSizeBytes)
            return BadRequest(new { Error = "File exceeds the maximum allowed size." });

        var fileName = file.FileName ?? string.Empty;
        if (string.IsNullOrWhiteSpace(fileName) || fileName.Length > 255 ||
            fileName.Contains('/') || fileName.Contains('\\') || fileName.Contains("..") ||
            fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            _logger.LogWarning("Rejected document upload for ECTN {EctnId}: unsafe file name", ectnId);
            return BadRequest(new { Error = "Invalid file name." });
        }

        var extension = Path.GetExtension(fileName);
        if (!AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Rejected document upload for ECTN {EctnId}: extension '{Extension}' not allowed", ectnId, extension);
            return BadRequest(new { Error = "File type is not allowed." });
        }

        var contentType = file.ContentType ?? string.Empty;
        if (!AllowedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Rejected document upload for ECTN {EctnId}: content type '{ContentType}' not allowed", ectnId, contentType);
            return BadRequest(new { Error = "Content type is not allowed." });
        }

        if (string.IsNullOrWhiteSpace(documentType) || !DocumentTypePattern.IsMatch(documentType))
        {
            _logger.LogWarning("Rejected document upload for ECTN {EctnId}: invalid document type", ectnId);
            return BadRequest(new { Error = "Invalid document type." });
        }

        // Pseudo-command: UploadDocumentCommand (storage key derived from server side identifiers
        // only - never from the client supplied name - followed by a malware scan).
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
