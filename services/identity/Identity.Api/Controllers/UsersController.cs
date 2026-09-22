using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/v1/identity/[controller]")]
// [Authorize]
public class UsersController : ControllerBase
{
    [HttpGet]
    // [RequirePermission("users.read")]
    public async Task<IActionResult> GetUsers()
    {
        // Must filter by ITenantContext automatically via EF Global Query Filters
        return Ok(new { Message = "Tenant isolated users retrieved" });
    }

    [HttpPost("{id}/suspend")]
    // [RequirePermission("users.update")]
    public async Task<IActionResult> SuspendUser(Guid id)
    {
        return Ok(new { Message = $"User {id} suspended" });
    }
}
