using DMS.Domain.DomainEvents;
using DMS.Domain.ValueObjects;

namespace DMS.Domain.Entities.Documents
{
    using Tags;
    public class DmsDocument : AggregateRoot
    {
        public string Title { get; private set; }
        public string? Content { get; private set; }
        public DateTime UploadDateTime { get; init; }
        public DateTime? ModificationDateTime { get; private set; } = null;
        public string? Path { get; private set; }
        public List<Tag>? Tags { get; set; } = new List<Tag>();
        public string FileExtension { get; private set; }
        public ProcessingStatus Status { get; private set; } = ProcessingStatus.NotStarted;

        public DmsDocument()
        {
        }

        private DmsDocument(string title, DateTime uploadDateTime, string? path, List<Tag>? tags,
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
            string? path, List<Tag>? tags, string documentType,
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

        public DmsDocument AddTag(Tag tag)
        {
            if (!Tags.Contains(tag))
                Tags.Add(tag);

            return this;
        }

        public DmsDocument UpdateTags(List<Tag> tags)
        {
            Tags = new List<Tag>();
            tags.ForEach(tag => AddTag(tag));
            this.ModificationDateTime = DateTime.UtcNow;
            AddDomainEventIfNotExists(new DocumentUpdatedDomainEvent(this));
            return this;
        }

        public DmsDocument RemoveTag(Tag tag)
        {
            Tags?.Remove(tag);
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