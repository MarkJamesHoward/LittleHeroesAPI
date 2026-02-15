using LittleHeroesAPI.Data;
using LittleHeroesAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LittleHeroesAPI.Controllers;

[Route("api/[controller]")]
public class GroupController : BaseApiController
{
    private readonly AppDbContext _db;

    public GroupController(AppDbContext db)
    {
        _db = db;
    }

    // GET api/Group
    [HttpGet]
    public async Task<IActionResult> GetGroupDetails()
    {
        var auth0Id = GetAuth0UserId();
        var parent = await _db.Parents
            .Include(p => p.Group)
                .ThenInclude(g => g.Parents)
            .FirstOrDefaultAsync(p => p.Auth0UserId == auth0Id);

        if (parent == null) return NotFound();

        var members = parent.Group.Parents.Select(p => new ParentDto
        {
            ID = p.Id,
            Email = p.Email,
            GroupName = parent.Group.Name
        }).ToList();

        return Ok(members);
    }

    // DELETE api/Group/RemoveGroupMember/{memberId}
    [HttpDelete("RemoveGroupMember/{memberId}")]
    public async Task<IActionResult> RemoveGroupMember(int memberId)
    {
        var auth0Id = GetAuth0UserId();
        var currentParent = await _db.Parents.FirstOrDefaultAsync(p => p.Auth0UserId == auth0Id);
        if (currentParent == null) return NotFound();

        var memberToRemove = await _db.Parents.FindAsync(memberId);
        if (memberToRemove == null) return NotFound();

        // Only allow removing members from your own group
        if (memberToRemove.GroupId != currentParent.GroupId)
            return Forbid();

        // Create a new group for the removed member
        var newGroup = new Models.Group { Name = memberToRemove.Email };
        _db.Groups.Add(newGroup);
        await _db.SaveChangesAsync();

        memberToRemove.GroupId = newGroup.Id;
        await _db.SaveChangesAsync();

        // Return updated group
        return await GetGroupDetails();
    }
}
