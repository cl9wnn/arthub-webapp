using MyORM;

namespace Persistence.Entities;

public class Reward
{
    [ColumnName("reward_id")]
    public int RewardId { get; set; }
    public string? Name { get; set; }
    public int Cost { get; set; }
}