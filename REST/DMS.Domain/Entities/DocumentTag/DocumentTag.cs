namespace DMS.Domain.Entities
{
    public class DocumentTag : Entity
    {
        public Guid DocumentId { get; set; }
        public DmsDocument Document { get; set; }
        public Guid TagId { get; set; }
        public Tag.Tag Tag { get; set; }
        protected DocumentTag() {}

        public DocumentTag(DmsDocument document, Tag.Tag tag)
        {
            Document = document;
            TagId = tag.Id;
            Tag = tag;
        }
    }
}