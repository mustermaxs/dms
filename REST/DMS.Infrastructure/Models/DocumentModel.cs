namespace DMS.Infrastructure.Models;

public class DocumentModel : BasePersistenceModel
{
    public string Title { get; private set; }
    public string? Content { get; private set; }
    public DateTime UploadDateTime { get; init; }
    public DateTime? ModificationDateTime { get; private set; } = null;
    public string? Path { get; private set; }
    public ICollection<DocumentTagModel>? Tags { get; set; } = new List<DocumentTagModel>();
    public FileTypeModel DocumentType { get; private set; }
    public ProcessingStatus Status { get; private set; } = ProcessingStatus.NotStarted;
    
    public DocumentModel(){}
}