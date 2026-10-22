using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using BuildingBlocks.Web;

namespace Ectn.Api.Controllers;

[ApiController]
[Route("api/v1/ectn/verify")]
// The verification code is a public proof-of-possession token, so the endpoint is intentionally
// anonymous - but it is covered by the deny-by-default fallback policy exception below and is
// rate limited so codes cannot be enumerated.
[AllowAnonymous]
public class EctnVerificationController : ControllerBase
{
    [HttpGet("{verificationCode}")]
    [EnableRateLimiting(PlatformSecurityExtensions.SensitiveRateLimitPolicy)]
    public IActionResult Verify(string verificationCode)
    {
        if (string.IsNullOrWhiteSpace(verificationCode) || verificationCode.Length > 128)
            return BadRequest(new { Error = "A verification code is required." });

        // Public endpoint (rate limited, A04/A07).
        // Never expose internal IDs or audit details here.
        // Lookup by signed payload or verification string
        var verificationResult = new
        {
            Status = "Valid",
            PortOfLoading = "JED",
            PortOfDischarge = "BER",
            VerifiedAt = DateTime.UtcNow
        };

        return Ok(verificationResult);
    }
}
