﻿namespace WebAPI.Models;

public class ArtistProfileModel: UserProfileModel
{
    public string? Fullname { get; init; }
    public string? ContactInfo { get; init; }
}