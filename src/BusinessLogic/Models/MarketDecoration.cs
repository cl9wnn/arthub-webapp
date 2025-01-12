using Persistence.Entities;
namespace BusinessLogic.Models;

public struct MarketDecoration
{
    public Decoration Decoration { get; set; }
    public bool IsBought { get; set; }
    public bool IsSelected { get; set; }
}