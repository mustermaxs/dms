

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
        IEnumerable<Tag>? tagsInDb = await tagRepository.GetAll();
        var existingTagValues = new HashSet<string>(tagsInDb.Select(dbTag => dbTag.Value));

        var newTags = tagDtos.Where(requestTag => 
            !existingTagValues.Contains(requestTag.Value));

        var alreadyExistingTagDtos = tagsInDb.Where(dbTag =>
            tagDtos.Any(requestTag => requestTag.Value == dbTag.Value));

        var alreadyExistingTags = new List<Tag>();
        foreach (var tagDto in alreadyExistingTagDtos)
        {
            var tag = await tagRepository.GetByValue(tagDto.Value);
            alreadyExistingTags.Add(tag);
        }

        var newTagsInDb = new List<Tag>();
        foreach (var tagDto in newTags)
        {
            var newTag = Tag.Create(tagDto.Label, tagDto.Value, tagDto.Color);
            var createdTag = await tagRepository.Create(newTag);  // Awaiting each call
            newTagsInDb.Add(createdTag);
        }

        return newTagsInDb.Concat(alreadyExistingTags).ToList();
    }
}
