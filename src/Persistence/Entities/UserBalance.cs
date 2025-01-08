using System.ComponentModel.DataAnnotations.Schema;
using MyORM;

namespace Persistence.Entities;

public class UserBalance
{
    [ColumnName("user_id")]
    public int UserId { get; set; }
    public int Balance { get; set; }
}