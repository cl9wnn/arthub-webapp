using MyORM;

namespace Persistence.Entities;

public class AccountReward
{
    [ColumnName("reward_id")]
    public int RewardId { get; set; }
    [ColumnName("reward_count")]
    public int RewardCount { get; set; }
}