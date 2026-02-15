namespace LittleHeroesAPI.Models;

public class LevelMadAchievement
{
    public int Id { get; set; }
    public int ChildId { get; set; }
    public Child Child { get; set; } = null!;
    public DateTime? DateOfLevelCompletion1 { get; set; }
}
