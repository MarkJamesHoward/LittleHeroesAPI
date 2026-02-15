namespace LittleHeroesAPI.DTOs;

// Matches the frontend BrowniePoints interface exactly
public class ChildDto
{
    public int Id { get; set; }
    public string ChildName { get; set; } = string.Empty;
    public int Points { get; set; }
    public int Level { get; set; }
    public int PointsNeeded { get; set; }
    public string Reward { get; set; } = string.Empty;
    public List<AvailableRewardDto> AvailableRewards { get; set; } = new();
    public bool Presenting { get; set; }
    public int PendingAdds { get; set; }
    public PetDto? Pet { get; set; }
    public string? Avatar { get; set; }
    public bool RemoveForAnimation { get; set; }
    public List<MyAchievementDto> MyAchievements { get; set; } = new();
    public int AchievementsTotal { get; set; }
}

public class PetDto
{
    public int Id { get; set; }
    public int Eyes { get; set; }
    public int SelectedEyeX { get; set; }
    public int SelectedEyeY { get; set; }
    public int Mouth { get; set; }
    public int SelectedMouthX { get; set; }
    public int SelectedMouthY { get; set; }
    public int Legs { get; set; }
    public int SelectedLegsX { get; set; }
    public int SelectedLegsY { get; set; }
    public int Silhouette { get; set; }
}

public class AvailableRewardDto
{
    public int Id { get; set; }
    public int BrowniePointsID { get; set; }
    public string Reward { get; set; } = string.Empty;
    public bool Used { get; set; }
    public bool BeingConsumed { get; set; }
}

public class MyAchievementDto
{
    public int Id { get; set; }
    public int AchievementsID { get; set; }
    public int Progress { get; set; }
}
