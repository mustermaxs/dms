

namespace DMS.Domain.Entities.DmsDocument
{
    using DMS.Domain.DomainEvents;
    using DMS.Domain.Entities.DmsDocument.ValueObjects;
    using DMS.Domain.ValueObjects;
    using DMS.Domain.Entities.Tags;
    
    public class DmsDocument : AggregateRoot
    {
        public string Title { get; private set; }
        public string? Content { get; private set; }
        public DateTime UploadDateTime { get; init; }
        public DateTime? ModificationDateTime { get; private set; } = null;
        public string? Path { get; private set; }
        private readonly List<Tag> _tags = new();
        public List<Tag> Tags
        {
            get => new List<Tag>(_tags.AsReadOnly());
            private set
            {
                _tags.Clear();
                _tags.AddRange(value);
            }
        }       
        public FileType DocumentType { get; private set; }
        public ProcessingStatus Status { get; private set; } = ProcessingStatus.NotStarted;

        public DmsDocument()
        {
        }

        private DmsDocument(
            Guid id,
            string title,
            DateTime uploadDateTime,
            string? path,
            List<Tag>? tags,
            FileType documentType,
            ProcessingStatus status)
        {
            Title = title;
            UploadDateTime = uploadDateTime;
            Path = path ?? Id.ToString();
            Tags = tags;
            DocumentType = documentType;
            Status = status;
        }  
        private DmsDocument(
            string title,
            DateTime uploadDateTime,
            string? path,
            List<Tag>? tags,
            FileType documentType,
            ProcessingStatus status)
        {
            Title = title;
            UploadDateTime = uploadDateTime;
            Path = path ?? Id.ToString();
            Tags = tags;
            DocumentType = documentType;
            Status = status;
        }

        public static DmsDocument Create(
            Guid id,
            string title,
            DateTime uploadDateTime,
            string? path, 
            List<Tag>? tags,
            FileType documentType,
            ProcessingStatus status)
        {
            return new DmsDocument(
                id,
                title,
                uploadDateTime,
                path,
                tags,
                documentType,
                status
            );
        }
        
        public static DmsDocument Create(
            string title,
            DateTime uploadDateTime,
            string? path, 
            List<Tag>? tags,
            FileType documentType,
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
            if (!_tags.Contains(tag))
                _tags.Add(tag);

            return this;
        }

        public DmsDocument UpdateTags(List<Tag> tags)
        {
            _tags.ForEach(tag => AddTag(tag));
            this.ModificationDateTime = DateTime.UtcNow;
            AddDomainEventIfNotExists(new DocumentUpdatedDomainEvent(this));
            return this;
        }

        public DmsDocument RemoveTag(Tag tag)
        {
            _tags.Remove(tag);
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
        
        public DmsDocument SetTags(List<Tag> tags)
        {
            Tags = tags;
            return this;
        }
    }
}