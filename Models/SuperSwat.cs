namespace LittleHeroesAPI.Models;

public class SuperSwat
{
    public int Id { get; set; }
    public int ChildId { get; set; }
    public Child Child { get; set; } = null!;
    public DateTime? Day1Date { get; set; }
    public DateTime? Day2Date { get; set; }
    public DateTime? Day3Date { get; set; }
    public DateTime? Day4Date { get; set; }
    public DateTime? Day5Date { get; set; }
    public int CompletedDays { get; set; }
}
