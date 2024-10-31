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
        public FileType DocumentType { get; private set; }
        public ProcessingStatus Status { get; private set; }

        public DmsDocument()
        {
        }

        private DmsDocument(string title, DateTime uploadDateTime, string? path, List<DocumentTag>? tags,
            FileType documentType,
            ProcessingStatus status)
        {
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
                title,
                uploadDateTime,
                path,
                tags,
                documentType,
                status
            );
        }

        public void AddTag(Tag.Tag tag)
        {
            var documentTag = new DocumentTag(this, tag);

            if (!Tags.Contains(documentTag))
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