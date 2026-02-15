using LittleHeroesAPI.Data;
using LittleHeroesAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace LittleHeroesAPI.Controllers;

[Route("api/[controller]")]
public class AvailableRewardsController : BaseApiController
{
    private readonly AppDbContext _db;
    private readonly ChildService _childService;

    public AvailableRewardsController(AppDbContext db, ChildService childService)
    {
        _db = db;
        _childService = childService;
    }

    // PUT api/AvailableRewards/SetRewardToUsed/{rewardId}
    [HttpPut("SetRewardToUsed/{rewardId}")]
    public async Task<IActionResult> SetRewardToUsed(int rewardId)
    {
        var reward = await _db.AvailableRewards.FindAsync(rewardId);
        if (reward == null) return NotFound();

        reward.Used = true;
        await _db.SaveChangesAsync();

        // Return updated children list
        var auth0Id = GetAuth0UserId();
        var children = await _childService.GetAllChildrenForParent(auth0Id);
        return Ok(children);
    }
}
