namespace DMS.Infrastructure.Models;

public class DocumentTagModel : BasePersistenceModel
{
    public Guid DocumentId { get; private set; }
    public DocumentModel Document { get; private set; }
    public Guid TagId { get; private set; }
    public TagModel Tag { get; private set; }
    protected DocumentTagModel() {}

    public DocumentTagModel(DocumentModel document, TagModel tag)
    {
        Document = document;
        DocumentId = document.Id;
        TagId = tag.Id;
        Tag = tag;
    }
}