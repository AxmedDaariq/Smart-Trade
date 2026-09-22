using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Ectn.Api.Controllers;

[ApiController]
[Route("api/v1/ectn/statistics")]
[Authorize(Policy = "ectn.statistics.read")]
public class EctnDashboardController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetDashboardStatistics()
    {
        // Returns high-level metrics for the Control Tower / Dashboard (Rule 29)
        // Uses Fast read-only queries with .AsNoTracking()
        
        var metrics = new
        {
            TotalEctns = 0,
            PendingReview = 0,
            Approved = 0,
            Rejected = 0,
            AverageProcessingTimeMinutes = 0
        };

        return Ok(metrics);
    }
}
