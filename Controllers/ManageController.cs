using LittleHeroesAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LittleHeroesAPI.Controllers;

[Route("[controller]")]
public class ManageController : BaseApiController
{
    private readonly AppDbContext _db;

    public ManageController(AppDbContext db)
    {
        _db = db;
    }

    // POST /Manage/SendInvite/{email}
    [HttpPost("SendInvite/{email}")]
    public async Task<IActionResult> SendInvite(string email)
    {
        var auth0Id = GetAuth0UserId();
        var currentParent = await _db.Parents.FirstOrDefaultAsync(p => p.Auth0UserId == auth0Id);
        if (currentParent == null) return NotFound();

        // Find the invited parent by email
        var invitedParent = await _db.Parents.FirstOrDefaultAsync(p => p.Email == email);
        if (invitedParent == null)
            return NotFound(new { message = "User not found. They need to register first." });

        // Move the invited parent to the current parent's group
        invitedParent.GroupId = currentParent.GroupId;
        await _db.SaveChangesAsync();

        return Ok(new { message = "Invite sent successfully" });
    }
}
