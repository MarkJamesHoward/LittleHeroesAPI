using LittleHeroesAPI.Data;
using LittleHeroesAPI.Models;
using LittleHeroesAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LittleHeroesAPI.Controllers;

[Route("api/[controller]")]
public class ChildrenController : BaseApiController
{
    private readonly AppDbContext _db;
    private readonly ChildService _childService;

    public ChildrenController(AppDbContext db, ChildService childService)
    {
        _db = db;
        _childService = childService;
    }

    // GET api/Children/all
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var auth0Id = GetAuth0UserId();
        var email = GetUserEmail();
        await _childService.GetOrCreateParent(auth0Id, email);
        var children = await _childService.GetAllChildrenForParent(auth0Id);
        return Ok(children);
    }

    // GET api/Children/ (alternate endpoint used by some components)
    [HttpGet]
    public async Task<IActionResult> GetAllAlt()
    {
        return await GetAll();
    }

    // GET api/Children/GetChild/{id}
    [HttpGet("GetChild/{id}")]
    public async Task<IActionResult> GetChild(int id)
    {
        var dto = await _childService.GetChildDto(id);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    // POST api/Children/{childName}
    [HttpPost("{childName}")]
    public async Task<IActionResult> AddChild(string childName)
    {
        var auth0Id = GetAuth0UserId();
        var email = GetUserEmail();
        var parent = await _childService.GetOrCreateParent(auth0Id, email);

        var child = new Child
        {
            GroupId = parent.GroupId,
            ChildName = childName,
            Points = 0,
            Level = 0,
            PointsNeeded = 100,
            Reward = "10 mins free play"
        };
        _db.Children.Add(child);
        await _db.SaveChangesAsync();

        // Create default pet
        var pet = new Pet
        {
            ChildId = child.Id,
            Eyes = 0, Mouth = 0, Legs = 0, Silhouette = 1,
            SelectedEyeX = 47, SelectedEyeY = 27,
            SelectedMouthX = 47, SelectedMouthY = 39,
            SelectedLegsX = 47, SelectedLegsY = 49
        };
        _db.Pets.Add(pet);

        // Create default achievements for the child
        var achievements = await _db.Achievements.ToListAsync();
        foreach (var achievement in achievements)
        {
            _db.MyAchievements.Add(new MyAchievement
            {
                ChildId = child.Id,
                AchievementsId = achievement.Id,
                Progress = 0
            });
        }

        // Create SuperSwat and LevelMad tracking
        _db.SuperSwats.Add(new SuperSwat { ChildId = child.Id, CompletedDays = 0 });
        _db.LevelMadAchievements.Add(new LevelMadAchievement { ChildId = child.Id });

        await _db.SaveChangesAsync();

        var children = await _childService.GetAllChildrenForParent(auth0Id);
        return Ok(children);
    }

    // DELETE api/Children/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveChild(int id)
    {
        var child = await _db.Children.FindAsync(id);
        if (child == null) return NotFound();

        _db.Children.Remove(child);
        await _db.SaveChangesAsync();
        return Ok();
    }

    // PUT api/Children/EditChildName/{id}/{newName}
    [HttpPut("EditChildName/{id}/{newName}")]
    public async Task<IActionResult> EditChildName(int id, string newName)
    {
        var child = await _db.Children.FindAsync(id);
        if (child == null) return NotFound();

        child.ChildName = newName;
        await _db.SaveChangesAsync();
        return Ok();
    }

    // PUT api/Children/EditReward/{id}/{reward}
    [HttpPut("EditReward/{id}/{reward}")]
    public async Task<IActionResult> EditReward(int id, string reward)
    {
        var child = await _db.Children.FindAsync(id);
        if (child == null) return NotFound();

        child.Reward = reward;
        await _db.SaveChangesAsync();
        return Ok();
    }
}
