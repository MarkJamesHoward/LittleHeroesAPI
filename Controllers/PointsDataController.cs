using LittleHeroesAPI.Data;
using LittleHeroesAPI.Models;
using LittleHeroesAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace LittleHeroesAPI.Controllers;

[Route("api/[controller]")]
public class PointsDataController : BaseApiController
{
    private readonly AppDbContext _db;
    private readonly ChildService _childService;

    public PointsDataController(AppDbContext db, ChildService childService)
    {
        _db = db;
        _childService = childService;
    }

    // GET api/PointsData/AddBrowniePointExtra/{childId}/{amount}
    [HttpGet("AddBrowniePointExtra/{childId}/{amount}")]
    public async Task<IActionResult> AddBrowniePointExtra(int childId, int amount)
    {
        var child = await _db.Children.FindAsync(childId);
        if (child == null) return NotFound();

        child.Points += amount;
        await _db.SaveChangesAsync();

        return Ok();
    }

    // GET api/PointsData/LevelUp/{childId}
    [HttpGet("LevelUp/{childId}")]
    public async Task<IActionResult> LevelUp(int childId)
    {
        var child = await _db.Children.FindAsync(childId);
        if (child == null) return NotFound();

        if (child.Points >= child.PointsNeeded)
        {
            child.Points -= child.PointsNeeded;
            child.Level++;

            // Create a new available reward
            _db.AvailableRewards.Add(new AvailableReward
            {
                ChildId = child.Id,
                Reward = child.Reward,
                Used = false
            });

            await _db.SaveChangesAsync();
        }

        return Ok();
    }
}
