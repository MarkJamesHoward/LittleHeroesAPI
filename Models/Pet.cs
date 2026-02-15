namespace LittleHeroesAPI.Models;

public class Pet
{
    public int Id { get; set; }
    public int ChildId { get; set; }
    public Child Child { get; set; } = null!;
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
