
namespace DMS.Infrastructure.Entities
{
    public class TagModel
    {
        public Guid Id { get; set; }
        public string Label { get; private set; }
        public string Value { get; private set; }
        public string Color { get; private set; }
        public ICollection<DocumentTagModel> DocumentTags { get; set; } = new List<DocumentTagModel>();

        public TagModel(){}

        public TagModel(Guid id, string label, string value, string color)
        {
            Id = id;
            Label = label;
            Value = value;
            Color = color;
        }
    }}