using LittleHeroesAPI.Data;
using LittleHeroesAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LittleHeroesAPI.Controllers;

[Route("api/[controller]")]
public class RewardsController : BaseApiController
{
    private readonly AppDbContext _db;

    public RewardsController(AppDbContext db)
    {
        _db = db;
    }

    // PUT api/Rewards/{rewardId}/{rewardText}
    [HttpPut("{rewardId}/{rewardText}")]
    public async Task<IActionResult> SaveReward(int rewardId, string rewardText)
    {
        var reward = await _db.AvailableRewards.FindAsync(rewardId);
        if (reward == null) return NotFound();

        reward.Reward = rewardText;
        await _db.SaveChangesAsync();
        return Ok(reward);
    }

    // DELETE api/Rewards/{rewardId}
    [HttpDelete("{rewardId}")]
    public async Task<IActionResult> RemoveReward(int rewardId)
    {
        var reward = await _db.AvailableRewards.FindAsync(rewardId);
        if (reward == null) return NotFound();

        _db.AvailableRewards.Remove(reward);
        await _db.SaveChangesAsync();
        return Ok();
    }

    // POST api/Rewards/{rewardText}/{childId}
    [HttpPost("{rewardText}/{childId}")]
    public async Task<IActionResult> AddReward(string rewardText, int childId)
    {
        var child = await _db.Children.FindAsync(childId);
        if (child == null) return NotFound();

        var reward = new AvailableReward
        {
            ChildId = childId,
            Reward = rewardText,
            Used = false
        };
        _db.AvailableRewards.Add(reward);
        await _db.SaveChangesAsync();

        return Ok(reward);
    }
}
