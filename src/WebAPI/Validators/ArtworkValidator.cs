using FluentValidation;
using Persistence.Entities;

namespace WebAPI.Validators;

public class ArtworkValidator: AbstractValidator<Artwork>
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

        RuleFor(a => a.ArtworkPath)
            .NotEmpty().WithMessage("ArtworkPath is required.");
    }
}