using FluentValidation;
using DMS.Domain.Entities;

public class DmsDocumentValidator : AbstractValidator<DmsDocument>
{
    public DmsDocumentValidator()
    {
        RuleFor(e => e.Title).NotNull();
        RuleFor(e => e.Title).NotEmpty();
        RuleFor(e => e.Title).MaximumLength(100);
    }
}