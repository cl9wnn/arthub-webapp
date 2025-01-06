namespace BusinessLogic.Models;

public class ProfileArtworkModel
{
    public int ArtworkId { get; set; }
    public string? ArtworkPath { get; set; }
    public int LikesCount { get; set; }
    public int ViewsCount { get; set; }
}