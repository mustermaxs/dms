using DMS.Infrastructure.ValueObjects;

namespace DMS.Infrastructure.Entities
{
    public class DocumentModel
    {
        public Guid Id { get; set; }
        public string Title { get; private set; }
        public string? Content { get; private set; }
        public DateTime UploadDateTime { get; init; }
        public DateTime? ModificationDateTime { get; private set; } = null;
        public string? Path { get; private set; }
        public ICollection<DocumentTagModel>? Tags { get; set; } = new List<DocumentTagModel>();
        public string FileExtension { get; private set; }
        public ProcessingStatusModel StatusModel { get; private set; } = ProcessingStatusModel.NotStarted;
    
        public DocumentModel(){}
    }
}
