using MyORM;

namespace Persistence.Entities;

public class ArtistDecoration
{
    [ColumnName("decoration_id")]
    public int DecorationId { get; set; }
    [ColumnName("user_id")]
    public int UserId { get; set; }
    [ColumnName("is_selected")]
    public bool IsSelected { get; set; }
}