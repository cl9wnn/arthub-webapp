using MyORM;

namespace Persistence.Entities;

public class UpgradeUser: User
{
    public string? Fullname { get; set; }
    [ColumnName("contact_info")]
    public string? ContactInfo { get; set; }
    public string? Summary { get; set; }
}