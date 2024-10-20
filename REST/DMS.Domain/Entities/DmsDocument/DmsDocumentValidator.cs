using FluentValidation;
using DMS.Domain.Entities;

public class DmsDocumentValidator : AbstractValidator<DmsDocument>
{
    public DmsDocumentValidator()
    {
        RuleFor(e => e.Title).NotNull();
        RuleFor(e => e.Title).NotEmpty();
        RuleFor(e => e.Title).MaximumLength(100);
        RuleFor(e => e.Title).Matches("^[a-zA-Z0-9_-]*$");
    }
}