using DMS.Application.DTOs;

namespace DMS.Domain.Entities.Tag
{

    public class Tag : Entity
    {
        public Guid Id { get; set; }
        public required string Label { get; set; }
        public string Value { get; set; }
        public string Color { get; set; }

        public Tag CreateTag(string label, string value, string color)
        {
            return new Tag
            {
                Id = Guid.NewGuid(),
                Label = label,
                Value = value,
                Color = color
            };
        }
    }
    public static class DocumentTagExtensions
    {
        public static TagDto ToDto(this Tag tag)
        {
            return new TagDto
            {
                Id = tag.Id,
                Label = tag.Label,
                Color = tag.Color,
                Value = tag.Value
            };
        }
    }
}