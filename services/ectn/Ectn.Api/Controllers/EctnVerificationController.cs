using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Ectn.Api.Controllers;

[ApiController]
[Route("api/v1/ectn/verify")]
public class EctnVerificationController : ControllerBase
{
    [HttpGet("{verificationCode}")]
    public async Task<IActionResult> Verify(string verificationCode)
    {
        // Public endpoint. Rate Limiting should be enforced via YARP or middleware (Rule 23 & 50)
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
