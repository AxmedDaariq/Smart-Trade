using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/v1/identity/[controller]")]
[Authorize]
public class TenantsController : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "identity.tenants.read")]
    public IActionResult GetTenants() => Problem(statusCode: StatusCodes.Status501NotImplemented, title: "Tenant listing is not implemented.");

    [HttpPost]
    [Authorize(Policy = "identity.tenants.manage")]
    public IActionResult CreateTenant() => Problem(statusCode: StatusCodes.Status501NotImplemented, title: "Tenant creation is not implemented.");

    [HttpPost("{id}/suspend")]
    [Authorize(Policy = "identity.tenants.manage")]
    public IActionResult SuspendTenant(Guid id) => Problem(statusCode: StatusCodes.Status501NotImplemented, title: "Tenant management is not implemented.");
}
