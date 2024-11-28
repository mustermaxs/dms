using DMS.Domain.DomainEvents;
using DMS.Domain.ValueObjects;

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
            return new DmsDocument(
                title,
                uploadDateTime,
                path,
                tags,
                documentType,
                status
            );
        }

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