using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LittleHeroesAPI.Controllers;

[ApiController]
[Authorize]
public abstract class BaseApiController : ControllerBase
{
    protected string GetAuth0UserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException("No user identifier in token");
    }

    protected string GetUserEmail()
    {
        return User.FindFirstValue(ClaimTypes.Email)
            ?? User.FindFirstValue("email")
            ?? "";
    }
}
