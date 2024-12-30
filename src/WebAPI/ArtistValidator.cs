using System.Text.RegularExpressions;
using FluentValidation;
using WebAPI.Models;

namespace WebAPI;

public class ArtistValidator: AbstractValidator<SignUpArtistModel>
{
    public ArtistValidator()
    {
        RuleFor(artist => artist.Fullname)
            .NotEmpty()
            .Matches("^[a-zA-Zа-яА-ЯёЁ '-]{3,30}$")
            .WithMessage("The name must be between 3 and 50 characters long and contain only letters, spaces, hyphens, or apostrophes.");
        RuleFor(user => user.ContactInfo)
            .NotEmpty()
            .Must(contactInfo => 
                Regex.IsMatch(contactInfo!, @"^(\+7|8)\d{10}$") || 
                Regex.IsMatch(contactInfo!, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$") || 
                Regex.IsMatch(contactInfo!, @"^(https?:\/\/)?([a-zA-Z0-9\-]+\.)+[a-zA-Z]{2,}\/?.*$")
            )
            .WithMessage("The contact info must be a valid phone number, email address, or URL.");
    }
}