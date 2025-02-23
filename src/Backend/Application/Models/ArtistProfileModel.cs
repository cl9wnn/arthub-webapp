using Persistence.Entities;

namespace Application.Models;

public class ArtistProfileModel: UserProfileModel
{
    public string? Fullname { get; init; }
    public string? ContactInfo { get; init; }
    public string? Summary { get; init; }
    public List<ProfileArtworkModel>? ProfileArts { get; init; }
    public List<AccountReward>? Rewards { get; init; }
}