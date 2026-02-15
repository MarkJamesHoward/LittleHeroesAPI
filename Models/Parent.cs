namespace LittleHeroesAPI.Models;

public class Parent
{
    public int Id { get; set; }
    public string Auth0UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int GroupId { get; set; }
    public Group Group { get; set; } = null!;
}
