using Persistence.Entities;

namespace WebAPI.Models;

public class ArtworkModel
{
    public string? Title { get; set; }
    public string? Category { get; set; }
    public string? Description {get; set;}
    public List<string>? Tags { get; set; }
    public FileModel? ArtFile { get; set; }
}