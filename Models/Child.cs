namespace LittleHeroesAPI.Models;

public class Child
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public Group Group { get; set; } = null!;
    public string ChildName { get; set; } = string.Empty;
    public int Points { get; set; }
    public int Level { get; set; }
    public int PointsNeeded { get; set; } = 100;
    public string Reward { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public Pet? Pet { get; set; }
    public List<AvailableReward> AvailableRewards { get; set; } = new();
    public List<MyAchievement> MyAchievements { get; set; } = new();
    public SuperSwat? SuperSwat { get; set; }
    public LevelMadAchievement? LevelMadAchievement { get; set; }
}
