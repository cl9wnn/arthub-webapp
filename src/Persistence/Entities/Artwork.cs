using MyORM;

namespace Persistence.Entities;

public class Artwork
{
    [ColumnName("artwork_id")]
    public int ArtworkId { get; set; }
    public string? Title { get; set; }
    public string? Category { get; set; }
    public string? Description {get; set;}
    [ColumnName("artwork_path")]
    public string? ArtworkPath { get; set; }
    [ColumnName("user_id")]
    public int UserId { get; set; }
}