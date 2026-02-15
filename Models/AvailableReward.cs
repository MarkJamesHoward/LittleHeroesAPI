namespace LittleHeroesAPI.Models;

public class AvailableReward
{
    public int Id { get; set; }
    public int ChildId { get; set; }
    public Child Child { get; set; } = null!;
    public string Reward { get; set; } = string.Empty;
    public bool Used { get; set; }
}
