using MyORM;

namespace Persistence.Entities;

public class SavingArt
{
    [ColumnName("user_id")]
    public int UserId { get; set; }
    [ColumnName("artwork_id")]
    public int ArtworkId { get; set; }
}