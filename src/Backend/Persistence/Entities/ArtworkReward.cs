using MyORM;

namespace Persistence.Entities;

public class ArtworkReward
{
    [ColumnName("artwork_id")]
    public int ArtworkId { get; set; }
    [ColumnName("reward_id")]
    public int RewardId { get; set; }
    [ColumnName("reward_count")]
    public int RewardCount {get; set;}
}