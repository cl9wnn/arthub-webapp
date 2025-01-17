using Persistence.Entities;

namespace Application.Models;

public struct MarketDecoration
{
    public Decoration Decoration { get; set; }
    public bool IsBought { get; set; }
    public bool IsSelected { get; set; }
}