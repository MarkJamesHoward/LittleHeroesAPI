namespace LittleHeroesAPI.Models;

public class MyAchievement
{
    public int Id { get; set; }
    public int ChildId { get; set; }
    public Child Child { get; set; } = null!;
    public int AchievementsId { get; set; }
    public Achievement Achievement { get; set; } = null!;
    public int Progress { get; set; }
}
