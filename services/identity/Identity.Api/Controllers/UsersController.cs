using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/v1/identity/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "identity.users.read")]
    public IActionResult GetUsers() => Problem(statusCode: StatusCodes.Status501NotImplemented, title: "User listing is not implemented.");

    [HttpPost("{id}/suspend")]
    [Authorize(Policy = "identity.users.manage")]
    public IActionResult SuspendUser(Guid id) => Problem(statusCode: StatusCodes.Status501NotImplemented, title: "User management is not implemented.");
}
