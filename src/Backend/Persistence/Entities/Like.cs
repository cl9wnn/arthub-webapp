using MyORM;

namespace Persistence.Entities;

public class Like
{
    [ColumnName("user_id")]
    public int UserId { get; set; }
    [ColumnName("artwork_id")]
    public int ArtworkId { get; set; }
    [ColumnName("created_at")]
    public DateTime CreatedAt { get; set; }
}