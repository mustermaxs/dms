using FluentValidation;

namespace DMS.Domain.Entities.Tag;

public class TagValidator : AbstractValidator<Tag>
{
    public TagValidator()
    {
        RuleFor(e => e.Label).NotNull();
        RuleFor(e => e.Value).NotNull();
        RuleFor(e => e.Label).NotEmpty();
        RuleFor(e => e.Value).NotEmpty();
        RuleFor(e => e.Label).MaximumLength(20);
        RuleFor(e => e.Value).MaximumLength(20);
        RuleFor(e => e.Label).Equal(e => e.Value.ToLower());
        RuleFor(e => e.Value).Equal(e => e.Label.ToLower());
        RuleFor(e => e.Label).Matches("^[a-z0-9_-]*$");
    }
}