using LittleHeroesAPI.Data;
using LittleHeroesAPI.DTOs;
using LittleHeroesAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LittleHeroesAPI.Services;

public class ChildService
{
    private readonly AppDbContext _db;

    public ChildService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Parent> GetOrCreateParent(string auth0UserId, string email)
    {
        var parent = await _db.Parents
            .Include(p => p.Group)
            .FirstOrDefaultAsync(p => p.Auth0UserId == auth0UserId);

        if (parent != null)
            return parent;

        var group = new Group { Name = email };
        _db.Groups.Add(group);
        await _db.SaveChangesAsync();

        parent = new Parent
        {
            Auth0UserId = auth0UserId,
            Email = email,
            GroupId = group.Id
        };
        _db.Parents.Add(parent);
        await _db.SaveChangesAsync();

        return parent;
    }

    public async Task<List<ChildDto>> GetAllChildrenForParent(string auth0UserId)
    {
        var parent = await _db.Parents.FirstOrDefaultAsync(p => p.Auth0UserId == auth0UserId);
        if (parent == null)
            return new List<ChildDto>();

        var children = await _db.Children
            .Where(c => c.GroupId == parent.GroupId)
            .Include(c => c.Pet)
            .Include(c => c.AvailableRewards)
            .Include(c => c.MyAchievements)
            .ToListAsync();

        return children.Select(MapToDto).ToList();
    }

    public async Task<ChildDto?> GetChildDto(int childId)
    {
        var child = await GetChildWithIncludes(childId);
        return child == null ? null : MapToDto(child);
    }

    public async Task<Child?> GetChildWithIncludes(int childId)
    {
        return await _db.Children
            .Include(c => c.Pet)
            .Include(c => c.AvailableRewards)
            .Include(c => c.MyAchievements)
            .FirstOrDefaultAsync(c => c.Id == childId);
    }

    public static ChildDto MapToDto(Child child)
    {
        return new ChildDto
        {
            Id = child.Id,
            ChildName = child.ChildName,
            Points = child.Points,
            Level = child.Level,
            PointsNeeded = child.PointsNeeded,
            Reward = child.Reward,
            Avatar = child.Avatar,
            Presenting = false,
            PendingAdds = 0,
            RemoveForAnimation = false,
            Pet = child.Pet == null ? null : new PetDto
            {
                Id = child.Pet.Id,
                Eyes = child.Pet.Eyes,
                SelectedEyeX = child.Pet.SelectedEyeX,
                SelectedEyeY = child.Pet.SelectedEyeY,
                Mouth = child.Pet.Mouth,
                SelectedMouthX = child.Pet.SelectedMouthX,
                SelectedMouthY = child.Pet.SelectedMouthY,
                Legs = child.Pet.Legs,
                SelectedLegsX = child.Pet.SelectedLegsX,
                SelectedLegsY = child.Pet.SelectedLegsY,
                Silhouette = child.Pet.Silhouette
            },
            AvailableRewards = child.AvailableRewards
                .Where(r => !r.Used)
                .Select(r => new AvailableRewardDto
                {
                    Id = r.Id,
                    BrowniePointsID = r.ChildId,
                    Reward = r.Reward,
                    Used = r.Used,
                    BeingConsumed = false
                }).ToList(),
            MyAchievements = child.MyAchievements.Select(a => new MyAchievementDto
            {
                Id = a.Id,
                AchievementsID = a.AchievementsId,
                Progress = a.Progress
            }).ToList(),
            AchievementsTotal = child.MyAchievements.Count
        };
    }
}
