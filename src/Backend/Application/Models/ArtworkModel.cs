﻿namespace Application.Models;

public class ArtworkModel
{
    public string? Title { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public List<string>? Tags { get; set; }
    public int UserId { get; set; }
}