namespace BusinessLogic.Models;

public class ArtworkPostModel
{
    public int ArtworkId { get; set; }
    public string? Title { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public string? ArtworkPath { get; set; }
    public List<string>? Tags { get; set; }
    public string? ProfileName { get; set; }
    public string? Fullname { get; set; }
    public string? AvatarPath { get; set; }
}