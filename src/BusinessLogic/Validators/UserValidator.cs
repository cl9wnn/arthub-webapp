using BusinessLogic.Models;
using FluentValidation;
using Persistence.Entities;
using WebAPI.Models;

namespace BusinessLogic.Validators;

public class UserValidator:AbstractValidator<UserModel>
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
            .WithMessage("The profile name must be between 3 and 15 characters long and contain only Latin letters," +
                         " numbers, dots, or underscores.");
    }
}