using System.ComponentModel.DataAnnotations.Schema;
using MyORM;

namespace Persistence.Entities;

public class ArtMetrics
{
    [ColumnName("artwork_id")]
    public int ArtworkId { get; set; }
    [ColumnName("likes_count")]
    public int LikesCount { get; set; }
    [ColumnName("views_count")]
    public int ViewsCount { get; set; }
}