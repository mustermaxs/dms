namespace DMS.Domain.Entities.Tags
{
    public class Tag : Entity
    {
        public string Label { get; private set; }
        public string Value { get; private set; }
        public string Color { get; private set; }

        public Tag(){}
        
        public Tag(string label, string value, string color)
        {
            Label = label;
            Value = value;
            Color = color;
        }

        public Tag(Guid id, string label, string value, string color)
        {
            Id = id;
            Label = label;
            Value = value;
            Color = color;
        }
    }
}