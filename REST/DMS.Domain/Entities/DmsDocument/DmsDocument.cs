using DMS.Application.DTOs;
using DMS.Domain.Entities.DomainEvents;
using DMS.Domain.ValueObjects;

namespace DMS.Domain.Entities
{
    public class DmsDocument : AggregateRoot
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime UploadDateTime { get; set; }
        public DateTime? ModificationDateTime { get; set; }
        public string? Path { get; set; }
        public List<DocumentTag>? Tags { get; set; } = new List<DocumentTag>();
        public FileType DocumentType { get; set; }
        public ProcessingStatus Status { get; set; }
        public DmsDocument() {}
        public DmsDocument(Guid id, string title, string content, DateTime uploadDateTime,
            DateTime modificationDateTime, string path, List<DocumentTag> tags, FileType documentType,
            ProcessingStatus status)
        {
            Id = id;
            Title = title;
            UploadDateTime = uploadDateTime;
            ModificationDateTime = modificationDateTime;
            Path = path;
            Tags = tags;
            DocumentType = documentType;
            Status = status;
        }

        public void AddTag(DocumentTag documentTag)
        {
            if (Tags.Contains(documentTag))
            {
                return;
            }
            Tags.Add(documentTag);
        }

        public void RemoveTag(DocumentTag documentTag)
        {
            Tags.Remove(documentTag);
        }
    }

    public static class DmsDocumentExtensions
    {
        public static DmsDocumentDto ToDto(this DmsDocument document)
        {
            return new DmsDocumentDto
            {
                Id = Guid.NewGuid(), Title = "Document 1.pdf",
                UploadDateTime = DateTime.Now,
                ModificationDateTime = DateTime.Now,
                Status = ProcessingStatus.Finished,
                Tags = [new TagDto{ Label = "contract", Color = "#FF0000", Value = "contract" }],
                DocumentType = new FileType(name: document.Title)
            };
        }
    }
}