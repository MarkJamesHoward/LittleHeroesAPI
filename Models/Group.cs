namespace LittleHeroesAPI.Models;

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Parent> Parents { get; set; } = new();
    public List<Child> Children { get; set; } = new();
}
