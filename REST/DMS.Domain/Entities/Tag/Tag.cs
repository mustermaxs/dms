using DMS.Domain.Entities.DomainEvents;

namespace DMS.Domain.Entities.Tag
{

    public class Tag : Entity
    {
        public string Label { get; private set; }
        public string Value { get; private set; }
        public string Color { get; private set; }
        public ICollection<DocumentTag> DocumentTags { get; set; } = new List<DocumentTag>();

        public Tag(){}

        public Tag(string label, string value, string color)
        {
            Label = label;
            Value = value;
            Color = color;
        }
    }
}