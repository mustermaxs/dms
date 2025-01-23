using DMS.Domain.Entities.DomainEvents;
using DMS.Domain.Exceptions;
using FluentValidation;

namespace DMS.Domain.Entities.Tag
{

    public class Tag : Entity
    {
        public string Label { get; private set; }
        public string Value { get; private set; }
        public string Color { get; private set; }
        public ICollection<DocumentTag> DocumentTags { get; set; } = new List<DocumentTag>();

        public Tag(){}

        private Tag(string label, string value, string color)
        {
            Label = label;
            Value = value;
            Color = color;
        }

        public static Tag Create(string label, string value, string color)
        {
            var tag = new Tag(label, value, color);
            var validationResult = Validator.Validate(tag);
            
            if (!validationResult.IsValid)
                throw new DomainEntityValidationException("Failed to create tag.", "Tag");
            
            return tag;
        }

        private static readonly AbstractValidator<Tag> Validator = new InlineValidator<Tag>()
        {
            validator => validator.RuleFor(e => e.Label).NotNull(),
            validator => validator.RuleFor(e => e.Value).NotNull(),
            validator => validator.RuleFor(e => e.Label).NotEmpty(),
            validator => validator.RuleFor(e => e.Value).NotEmpty(),
            validator => validator.RuleFor(e => e.Label).MaximumLength(20),
            validator => validator.RuleFor(e => e.Value).MaximumLength(20),
            validator => validator.RuleFor(e => e.Label.ToLower()).Equal(e => e.Value.ToLower()),
            validator => validator.RuleFor(e => e.Value.ToLower()).Equal(e => e.Label.ToLower()),
            validator => validator.RuleFor(e => e.Label).Matches("^[a-zA-Z0-9_-]*$")
        };

    }
}