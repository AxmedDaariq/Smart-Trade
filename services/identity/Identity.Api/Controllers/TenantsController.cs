using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/v1/identity/[controller]")]
// [Authorize(Policy = "RequirePlatformAdmin")] // Protect this in real implementation
public class TenantsController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTenants()
    {
        return Ok(new { Message = "Tenants retrieved successfully" });
    }

    [HttpPost]
    public async Task<IActionResult> CreateTenant([FromBody] object command)
    {
        return Ok(new { Message = "Tenant created successfully" });
    }

    [HttpPost("{id}/suspend")]
    public async Task<IActionResult> SuspendTenant(Guid id)
    {
        return Ok(new { Message = $"Tenant {id} suspended" });
    }
}
