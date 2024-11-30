namespace DMS.Infrastructure.Entities;

public class DocumentTagModel
{
    public Guid Id { get; set; }
    public Guid DocumentId { get;  set; }
    public DocumentModel DocumentModels { get;  set; } // TODO rename to singular
    public Guid TagId { get;  set; }
    public TagModel TagModels { get;  set; } // TODO rename to singular
    public DocumentTagModel() {}

}