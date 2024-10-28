namespace DMS.Domain.Entities;
using FluentValidation;

public class DocumentTagValidator : AbstractValidator<DocumentTag>
{
    public DocumentTagValidator()
    {
        RuleFor(e => e.DocumentId).NotNull();
        RuleFor(e => e.TagId).NotNull();
    }
}