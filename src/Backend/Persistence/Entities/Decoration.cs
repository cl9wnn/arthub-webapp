using MyORM;

namespace Persistence.Entities;

public class Decoration
{
    [ColumnName("decoration_id")]
    public int DecorationId { get; set; }
    public string? Name { get; set; }
    public int Cost  { get; set; }
    [ColumnName("type_name")]
    public string? TypeName { get; set; }
}