namespace LittleHeroesAPI.DTOs;

// Matches the frontend Parent interface
public class ParentDto
{
    public int ID { get; set; }
    public string Email { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
}
