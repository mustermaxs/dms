namespace DMS.Domain.Entities
{
    public class DocumentTag : Entity
    {
        public Guid DocumentId { get; private set; }
        public DmsDocument Document { get; private set; }
        public Guid TagId { get; private set; }
        public Tag.Tag Tag { get; private set; }
        protected DocumentTag() {}

        public DocumentTag(DmsDocument document, Tag.Tag tag)
        {
            Document = document;
            TagId = tag.Id;
            Tag = tag;
        }
    }
}