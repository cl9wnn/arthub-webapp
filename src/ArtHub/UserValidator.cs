using ArtHub.Entities;
using FluentValidation;

namespace ArtHub;

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
    }
}