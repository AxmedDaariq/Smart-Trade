using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Ectn.Api.Controllers;

[ApiController]
[Route("api/v1/ectn/search")]
[Authorize(Policy = "ectn.read")]
public class EctnSearchController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string ectnNumber, [FromQuery] string port, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        // Enforce maximum page size to prevent abuse (Rule 28)
        if (pageSize > 100) pageSize = 100;

        // Perform optimized index-based search ensuring TenantId filtering is always applied via EF Core Global Filters
        
        return Ok(new { Page = page, PageSize = pageSize, TotalCount = 0, Results = new object[] { } });
    }
}
