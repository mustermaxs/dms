namespace DMS.Domain.Entities
{
    public class DocumentTag : Entity
    {
        public Guid DocumentId { get; set; }
        public DmsDocument Document { get; set; }
        public Guid TagId { get; set; }
        public Tag.Tag Tag { get; set; }
        protected DocumentTag() {}

        private DocumentTag(DmsDocument document, Tag.Tag tag)
        {
            DocumentId = document.Id;
            Document = document;
            TagId = tag.Id;
            Tag = tag;
        }

        public static DocumentTag Create(Tag.Tag tag, DmsDocument document)
        {
            return new DocumentTag(
                document,
                tag
            );
        }
    }
}