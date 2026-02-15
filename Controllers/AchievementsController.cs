using LittleHeroesAPI.Data;
using LittleHeroesAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LittleHeroesAPI.Controllers;

[Route("api/[controller]")]
public class AchievementsController : BaseApiController
{
    private readonly AppDbContext _db;

    public AchievementsController(AppDbContext db)
    {
        _db = db;
    }

    // GET api/Achievements/
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var achievements = await _db.Achievements
            .Select(a => new AchievementDto
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                Progress = 0
            })
            .ToListAsync();

        return Ok(achievements);
    }

    // GET api/Achievements/GetSuperSwatAddDaySuccess/{childId}
    [HttpGet("GetSuperSwatAddDaySuccess/{childId}")]
    public async Task<IActionResult> GetSuperSwatAddDaySuccess(int childId)
    {
        var superSwat = await _db.SuperSwats.FirstOrDefaultAsync(s => s.ChildId == childId);
        if (superSwat == null)
            return Ok(new SuperSwatDto { BrowniePointsID = childId, CompletedDays = 0 });

        return Ok(new SuperSwatDto
        {
            ID = superSwat.Id,
            BrowniePointsID = superSwat.ChildId,
            Day1Date = superSwat.Day1Date?.ToString("o"),
            Day2Date = superSwat.Day2Date?.ToString("o"),
            Day3Date = superSwat.Day3Date?.ToString("o"),
            Day4Date = superSwat.Day4Date?.ToString("o"),
            Day5Date = superSwat.Day5Date?.ToString("o"),
            CompletedDays = superSwat.CompletedDays
        });
    }

    // GET api/Achievements/SuperSwatAddDaySuccess/{childId}/{day}/{date}
    [HttpGet("SuperSwatAddDaySuccess/{childId}/{day}/{date}")]
    public async Task<IActionResult> SuperSwatAddDaySuccess(int childId, int day, string date)
    {
        var superSwat = await _db.SuperSwats.FirstOrDefaultAsync(s => s.ChildId == childId);
        if (superSwat == null)
        {
            superSwat = new Models.SuperSwat { ChildId = childId, CompletedDays = 0 };
            _db.SuperSwats.Add(superSwat);
        }

        var parsedDate = DateTime.TryParse(date, out var dt) ? dt : DateTime.UtcNow;

        switch (day)
        {
            case 1: superSwat.Day1Date = parsedDate; break;
            case 2: superSwat.Day2Date = parsedDate; break;
            case 3: superSwat.Day3Date = parsedDate; break;
            case 4: superSwat.Day4Date = parsedDate; break;
            case 5: superSwat.Day5Date = parsedDate; break;
        }
        superSwat.CompletedDays = day;

        await _db.SaveChangesAsync();
        return Ok();
    }

    // GET api/Achievements/ResetSuperSwatAddDaySuccess/{childId}
    [HttpGet("ResetSuperSwatAddDaySuccess/{childId}")]
    public async Task<IActionResult> ResetSuperSwatAddDaySuccess(int childId)
    {
        var superSwat = await _db.SuperSwats.FirstOrDefaultAsync(s => s.ChildId == childId);
        if (superSwat != null)
        {
            superSwat.Day1Date = null;
            superSwat.Day2Date = null;
            superSwat.Day3Date = null;
            superSwat.Day4Date = null;
            superSwat.Day5Date = null;
            superSwat.CompletedDays = 0;
            await _db.SaveChangesAsync();
        }

        return Ok();
    }

    // GET api/Achievements/SetAchivementProgress/{childId}/{achievementId}/{progress}
    [HttpGet("SetAchivementProgress/{childId}/{achievementId}/{progress}")]
    public async Task<IActionResult> SetAchievementProgress(int childId, int achievementId, int progress)
    {
        var myAchievement = await _db.MyAchievements
            .FirstOrDefaultAsync(a => a.ChildId == childId && a.AchievementsId == achievementId);

        if (myAchievement == null)
        {
            myAchievement = new Models.MyAchievement
            {
                ChildId = childId,
                AchievementsId = achievementId,
                Progress = progress
            };
            _db.MyAchievements.Add(myAchievement);
        }
        else
        {
            myAchievement.Progress = progress;
        }

        await _db.SaveChangesAsync();
        return Ok();
    }

    // GET api/Achievements/GetLevelMadProgress/{childId}
    [HttpGet("GetLevelMadProgress/{childId}")]
    public async Task<IActionResult> GetLevelMadProgress(int childId)
    {
        var levelMad = await _db.LevelMadAchievements.FirstOrDefaultAsync(l => l.ChildId == childId);
        if (levelMad == null)
            return Ok(new LevelMadAchievementDto { BrowniePointsID = childId });

        return Ok(new LevelMadAchievementDto
        {
            Id = levelMad.Id,
            BrowniePointsID = levelMad.ChildId,
            DateOfLevelCompletion1 = levelMad.DateOfLevelCompletion1?.ToString("o")
        });
    }

    // GET api/Achievements/SetLevelMadProgressServer/{childId}/{date}
    [HttpGet("SetLevelMadProgressServer/{childId}/{date}")]
    public async Task<IActionResult> SetLevelMadProgressServer(int childId, string date)
    {
        var levelMad = await _db.LevelMadAchievements.FirstOrDefaultAsync(l => l.ChildId == childId);
        if (levelMad == null)
        {
            levelMad = new Models.LevelMadAchievement { ChildId = childId };
            _db.LevelMadAchievements.Add(levelMad);
        }

        levelMad.DateOfLevelCompletion1 = DateTime.TryParse(date, out var dt) ? dt : DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok();
    }

    // GET api/Achievements/CheckForMakeAMonster/{childId}
    [HttpGet("CheckForMakeAMonster/{childId}")]
    public async Task<IActionResult> CheckForMakeAMonster(int childId)
    {
        // Check if the child has customized their pet (any non-default values)
        var pet = await _db.Pets.FirstOrDefaultAsync(p => p.ChildId == childId);
        if (pet == null)
            return Ok(new { completed = false });

        bool customized = pet.Eyes != 0 || pet.Mouth != 0 || pet.Legs != 0;

        if (customized)
        {
            // Achievement ID 4 = Make a Monster
            var myAchievement = await _db.MyAchievements
                .FirstOrDefaultAsync(a => a.ChildId == childId && a.AchievementsId == 4);
            if (myAchievement != null && myAchievement.Progress < 100)
            {
                myAchievement.Progress = 100;
                await _db.SaveChangesAsync();
            }
        }

        return Ok(new { completed = customized });
    }
}
