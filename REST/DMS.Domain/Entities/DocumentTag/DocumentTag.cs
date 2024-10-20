namespace DMS.Domain.Entities
{

    public class DocumentTag : Entity
    {
        public Guid DocumentId { get; set; }
        public DmsDocument Document { get; set; }
        public Guid TagId { get; set; }
        public Tag.Tag Tag { get; set; }
    }
}