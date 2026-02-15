namespace LittleHeroesAPI.DTOs;

public class AchievementDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Progress { get; set; }
}

public class SuperSwatDto
{
    public int ID { get; set; }
    public int BrowniePointsID { get; set; }
    public string? Day1Date { get; set; }
    public string? Day2Date { get; set; }
    public string? Day3Date { get; set; }
    public string? Day4Date { get; set; }
    public string? Day5Date { get; set; }
    public int CompletedDays { get; set; }
}

public class LevelMadAchievementDto
{
    public int Id { get; set; }
    public int BrowniePointsID { get; set; }
    public string? DateOfLevelCompletion1 { get; set; }
}
