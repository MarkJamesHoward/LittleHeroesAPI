using LittleHeroesAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LittleHeroesAPI.Controllers;

[Route("api/[controller]")]
public class AvatarController : BaseApiController
{
    private readonly AppDbContext _db;

    public AvatarController(AppDbContext db)
    {
        _db = db;
    }

    // PUT api/Avatar/{childName}/{img}
    [HttpPut("{childName}/{img}")]
    public async Task<IActionResult> SetAvatar(string childName, string img)
    {
        var child = await _db.Children.FirstOrDefaultAsync(c => c.ChildName == childName);
        if (child == null) return NotFound();

        child.Avatar = img;
        await _db.SaveChangesAsync();
        return Ok();
    }
}
