using DMS.Domain.DomainEvents;
using DMS.Domain.ValueObjects;

namespace DMS.Domain.Entities
{
    public class DmsDocument : AggregateRoot
    {
        public Guid Id { get; init; }
        public string Title { get; private set; }
        public string? Content { get; private set; }
        public DateTime UploadDateTime { get; init; }
        public DateTime? ModificationDateTime { get; private set; } = null;
        public string? Path { get; private set; }
        public List<DocumentTag>? Tags { get; set; } = new List<DocumentTag>();
        public FileType DocumentType { get; private set; }
        public ProcessingStatus Status { get; private set; }
        public DmsDocument() {}

        private DmsDocument(Guid id, string title, DateTime uploadDateTime, string? path, List<DocumentTag>? tags, FileType documentType,
            ProcessingStatus status)
        {
            Id = id;
            Title = title;
            UploadDateTime = uploadDateTime;
            Path = path;
            Tags = tags;
            DocumentType = documentType;
            Status = status;
        }

        public static DmsDocument Create(string title, DateTime uploadDateTime,
            string? path, List<DocumentTag>? tags, FileType documentType,
            ProcessingStatus status)
        {
            return new DmsDocument(
                Guid.NewGuid(),
                title,
                uploadDateTime,
                path,
                tags,
                documentType,
                status
            );
        }

        public void AddTag(DocumentTag documentTag)
        {
            if (Tags.Contains(documentTag))
            {
                return;
            }
            Tags.Add(documentTag);
        }
        
        public void UpdateTags(List<DocumentTag> tags)
        {
            Tags = tags;
            this.ModificationDateTime = DateTime.UtcNow;
            AddDomainEvent(new DocumentTagsUpdatedDomainEvent(this));
        }

        public void RemoveTag(DocumentTag documentTag)
        {
            Tags.Remove(documentTag);
        }

        public void UpdateContent(string content)
        {
            Content = content;
        }
    }
}