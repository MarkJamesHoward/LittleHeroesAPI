using LittleHeroesAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LittleHeroesAPI.Controllers;

[Route("api/[controller]")]
public class PetController : BaseApiController
{
    private readonly AppDbContext _db;

    public PetController(AppDbContext db)
    {
        _db = db;
    }

    // PUT api/Pet/CustomizePet/{childId}/{eyes}/{mouth}/{legs}/{silhouette}/{mx}/{my}/{ex}/{ey}/{lx}/{ly}
    [HttpPut("CustomizePet/{childId}/{eyes}/{mouth}/{legs}/{silhouette}/{mx}/{my}/{ex}/{ey}/{lx}/{ly}")]
    public async Task<IActionResult> CustomizePet(
        int childId, int eyes, int mouth, int legs, int silhouette,
        int mx, int my, int ex, int ey, int lx, int ly)
    {
        var pet = await _db.Pets.FirstOrDefaultAsync(p => p.ChildId == childId);
        if (pet == null) return NotFound();

        pet.Eyes = eyes;
        pet.Mouth = mouth;
        pet.Legs = legs;
        pet.Silhouette = silhouette;
        pet.SelectedMouthX = mx;
        pet.SelectedMouthY = my;
        pet.SelectedEyeX = ex;
        pet.SelectedEyeY = ey;
        pet.SelectedLegsX = lx;
        pet.SelectedLegsY = ly;

        await _db.SaveChangesAsync();
        return Ok();
    }
}
