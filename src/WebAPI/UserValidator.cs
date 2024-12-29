using System.Data;
using System.Text.RegularExpressions;
using FluentValidation;
using Persistence.Entities;
using WebAPI.Models;

namespace WebAPI;

public class UserValidator:AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(user => user.Login)
            .NotEmpty()
            .Matches("^[a-zA-z0-9_]{5,20}$")
            .WithMessage("Login must be between 5 and 20 characters.");
        RuleFor(user => user.Password)
            .NotEmpty()
            .Matches(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")
            .WithMessage("Password must be between 8 and 20 characters, at least one digit, special symbol, and upper case letter.");
        RuleFor(user => user.ProfileName)
            .NotEmpty()
            .Matches("^[a-zA-Z0-9_.]{3,15}$")
            .WithMessage("The profile name must be between 3 and 15 characters long and contain only Latin letters, numbers, dots, or underscores.");
        RuleFor(user => user.RealName)
            .NotEmpty()
            .Matches("^[a-zA-Zа-яА-ЯёЁ '-]{3,30}$")
            .WithMessage("The name must be between 3 and 50 characters long and contain only letters, spaces, hyphens, or apostrophes.");
        RuleFor(user => user.ContactInfo)
            .NotEmpty()
            .Must(contactInfo => 
                    Regex.IsMatch(contactInfo, @"^(\+7|8)\d{10}$") || 
                    Regex.IsMatch(contactInfo, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$") || 
                    Regex.IsMatch(contactInfo, @"^(https?:\/\/)?([a-zA-Z0-9\-]+\.)+[a-zA-Z]{2,}\/?.*$")
            )
            .WithMessage("The contact info must be a valid phone number, email address, or URL.");
    }
}