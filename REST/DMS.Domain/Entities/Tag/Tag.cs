using DMS.Domain.Entities.DomainEvents;

namespace DMS.Domain.Entities.Tag
{

    public class Tag : Entity
    {
        public Guid Id { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public string Color { get; set; }
        public Tag(){}

        public Tag(string label, string value, string color)
        {
            Id = Guid.NewGuid();
            Label = label;
            Value = value;
            Color = color;
        }
    }
}