using DMS.Domain.Entities.DomainEvents;

namespace DMS.Domain.Entities.Tag
{

    public class Tag : Entity
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public string Color { get; set; }
        public Tag(){}

        public Tag(string label, string value, string color)
        {
            Label = label;
            Value = value;
            Color = color;
        }
    }
}