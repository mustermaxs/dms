using FluentValidation;
using DMS.Domain.Entities;

public class DmsDocumentValidator : AbstractValidator<DmsDocument>
{
    public DmsDocumentValidator()
    {
        RuleFor(e => e.Title).NotNull()
            .WithMessage("Document title is invalid. It should not be null.");
        RuleFor(e => e.Title).NotEmpty()
            .WithMessage("Document title is invalid. It should not be empty.");
        RuleFor(e => e.Title).MaximumLength(50)
            .WithMessage("Document title is invalid. It should be less than 50 characters.");
        RuleFor(e => e.Title).Matches("^[ a-zA-Z0-9_-]+")
            .WithMessage("Document title is invalid. ");
    }
}