using DMS.Domain.DomainEvents;
using DMS.Domain.Exceptions;
using DMS.Domain.ValueObjects;
using FluentValidation;

namespace DMS.Domain.Entities
{
    public class DmsDocument : AggregateRoot
    {
        public string Title { get; private set; }
        public string? Content { get; private set; }
        public DateTime UploadDateTime { get; init; }
        public DateTime? ModificationDateTime { get; private set; } = null;
        public string? Path { get; private set; }
        public ICollection<DocumentTag>? Tags { get; set; } = new List<DocumentTag>();
        public string FileExtension { get; private set; }
        public ProcessingStatus Status { get; private set; } = ProcessingStatus.NotStarted;

        public DmsDocument()
        {
        }

        private DmsDocument(string title, DateTime uploadDateTime, string? path, List<DocumentTag>? tags,
            string fileExtension,
            ProcessingStatus status)
        {
            Title = title;
            UploadDateTime = uploadDateTime;
            Path = path ?? Id.ToString();
            Tags = tags;
            FileExtension = fileExtension;
            Status = status;
        }

        public static DmsDocument Create(string title, DateTime uploadDateTime,
            string? path, List<DocumentTag>? tags, string documentType,
            ProcessingStatus status)
        {
            var document = new DmsDocument(
                title,
                uploadDateTime,
                path,
                tags,
                documentType,
                status
            );
            
            var validationResult = Validator.Validate(document);
            if (!validationResult.IsValid)
                throw new DomainEntityValidationException("Failed to create document.", documentType);
            
            return document;
        }

        private static readonly AbstractValidator<DmsDocument> Validator = new InlineValidator<DmsDocument>()
        {
            validator => validator.RuleFor(e => e.Title).NotNull()
                .WithMessage("Document title is invalid. It should not be null."),
            validator => validator.RuleFor(e => e.Title).NotEmpty()
                .WithMessage("Document title is invalid. It should not be empty."),
            validator => validator.RuleFor(e => e.Title).MaximumLength(50)
                .WithMessage("Document title is invalid. It should be less than 50 characters."),
            validator => validator.RuleFor(e => e.Title).Matches("^[ a-zA-Z0-9_-]+")
                .WithMessage("Document title is invalid. "),
            validator => validator.RuleFor(e => e.FileExtension).NotNull(),
            validator => validator.RuleFor(e => e.FileExtension).Matches("pdf")
                .WithMessage("Document file extension is invalid.")
        };

        public DmsDocument AddTag(Tag.Tag tag)
        {
            var documentTag = new DocumentTag(this, tag);

            if (!Tags.Contains(documentTag))
                Tags.Add(documentTag);

            return this;
        }

        public DmsDocument UpdateTags(List<Tag.Tag> tags)
        {
            Tags = new List<DocumentTag>();
            tags.ForEach(tag => AddTag(tag));
            this.ModificationDateTime = DateTime.UtcNow;
            AddDomainEventIfNotExists(new DocumentUpdatedDomainEvent(this));
            return this;
        }

        public DmsDocument RemoveTag(Tag.Tag tag)
        {
            var documentTag = new DocumentTag(this, tag);
            Tags.Remove(documentTag);
            ModificationDateTime = DateTime.UtcNow;

            AddDomainEventIfNotExists(new DocumentUpdatedDomainEvent(this));

            return this;
        }

        public DmsDocument UpdateTitle(string title)
        {
            Title = title;
            ModificationDateTime = DateTime.UtcNow;

            AddDomainEventIfNotExists(new DocumentUpdatedDomainEvent(this));

            return this;
        }

        public DmsDocument UpdateContent(string content)
        {
            Content = content;
            ModificationDateTime = DateTime.UtcNow;
            Status = ProcessingStatus.Finished;
            AddDomainEventIfNotExists(new DocumentUpdatedDomainEvent(this));

            return this;
        }

        public DmsDocument UpdatePath(string path)
        {
            Path = path;
            ModificationDateTime = DateTime.UtcNow;
            AddDomainEventIfNotExists(new DocumentUpdatedDomainEvent(this));
            
            return this;
        }
        
        public DmsDocument SetStatus(ProcessingStatus status)
        {
            Status = status;
            return this;
        }
    }
}