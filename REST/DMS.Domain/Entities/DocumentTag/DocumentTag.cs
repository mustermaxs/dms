using DMS.Application.DTOs;

namespace DMS.Domain.Entities
{

    public class DocumentTag : Entity
    {
        public Guid DocumentId { get; set; }
        public DmsDocument Document { get; set; }
        public Guid TagId { get; set; }
        public Tag.Tag Tag { get; set; }
    }

    public static class DocumentTagExtensions
    {
        public static TagDto ToDto(this DocumentTag documentTag)
        {
            return new TagDto
            {
                Id = documentTag.TagId,
                Label = documentTag.Tag.Label,
                Color = documentTag.Tag.Color,
                Value = documentTag.Tag.Value
            };
        }
    }
}