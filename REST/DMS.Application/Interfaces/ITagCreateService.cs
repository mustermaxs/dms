using DMS.Domain.Entities.Tags;

namespace DMS.Infrastructure.Services;

public interface ITagCreateService
{
    public Task<List<Tag>> CreateOrGetTagsFromTagDtos(List<Tag> tags);
}