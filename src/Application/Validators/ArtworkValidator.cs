using Application.Models;
using FluentValidation;

namespace Application.Validators;

public class ArtworkValidator: AbstractValidator<ArtworkModel>
{
    public ArtworkValidator()
    {
        RuleFor(a => a.Title)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Title must not exceed 100 characters.");

        RuleFor(a => a.Category)
            .NotEmpty()
            .Must(category => new[] { "portrait", "landscape", "anime", "abstraction" }.Contains(category))
            .WithMessage("Category must be one of the predefined values: Painting, Drawing, Sculpture, Photography.");

        RuleFor(a => a.Description)
            .MaximumLength(300)
            .WithMessage("Description must not exceed 300 characters.");
    }
}
