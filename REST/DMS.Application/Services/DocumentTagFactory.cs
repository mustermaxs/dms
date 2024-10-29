

using DMS.Application.DTOs;
using DMS.Application.Interfaces;
using DMS.Domain.Entities.Tag;
using DMS.Domain.IRepositories;
using DMS.Domain.Services;

namespace DMS.Infrastructure.Services;

public class DocumentTagFactory(
    ITagRepository tagRepository) : IDocumentTagFactory
{
    public async Task<List<Tag>> CreateOrGetTagsFromTagDtos(List<TagDto> tagDtos)
    {
        // Get all tags from the database
        IEnumerable<Tag>? tagsInDb = await tagRepository.GetAll();
        var existingTagValues = new HashSet<string>(tagsInDb.Select(dbTag => dbTag.Value));

        // Separate new tags from existing tags
        var newTags = tagDtos
            .Where(requestTag => requestTag.Id == Guid.Empty)
            .ToList();

        var alreadyExistingTagDtos = tagDtos
            .Where(requestTag => requestTag.Id != Guid.Empty);

        // Process existing tags one by one (sequentially)
        var alreadyExistingTags = new List<Tag>();
        foreach (var tagDto in alreadyExistingTagDtos)
        {
            var tag = await tagRepository.GetByValue(tagDto.Value);
            alreadyExistingTags.Add(tag);
        }

        // Process new tags one by one (sequentially)
        var newTagsInDb = new List<Tag>();
        foreach (var tagDto in newTags)
        {
            var newTag = new Tag(tagDto.Label, tagDto.Value, tagDto.Color);
            var createdTag = await tagRepository.Create(newTag);  // Awaiting each call
            newTagsInDb.Add(createdTag);
        }

        // Return the combined result of new and existing tags
        return newTagsInDb.Concat(alreadyExistingTags).ToList();
    }
}
