namespace BusinessLogic.Models;

public class GalleryArtworkModel
{
    
    public int ArtworkId { get; set; }
    public string? Title { get; set; }
    
    public string? Category { get; set; }
    public List<string>? Tags { get; set; }
    public string? ArtworkPath { get; set; }
    public string? ProfileName { get; set; }
    public string? AvatarPath { get; set; }
    public int LikesCount { get; set; }
    public int ViewsCount { get; set; }
}