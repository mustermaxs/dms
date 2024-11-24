using DMS.Application.DTOs;
using DMS.Application.Interfaces;
using DMS.Domain.Entities.Tags;
using DMS.Domain.IRepositories;

namespace DMS.Infrastructure.Services;

public class TagCreateService(
    ITagRepository tagRepository) : ITagCreateService
{
    public async Task<List<Tag>> CreateOrGetTagsFromTagDtos(List<Tag> tags)
    {
        IEnumerable<Tag>? tagsInDb = await tagRepository.GetAll();
        var existingTagValues = new HashSet<string>(tagsInDb.Select(dbTag => dbTag.Value));

        
        var newTags = tags.Where(requestTag => 
            !existingTagValues.Contains(requestTag.Value));

        var alreadyExistingTagDtos = tagsInDb.Where(dbTag =>
            tags.Any(requestTag => requestTag.Value == dbTag.Value));

        
        var alreadyExistingTags = new List<Tag>();
        foreach (var tagDto in alreadyExistingTagDtos)
        {
            var tag = await tagRepository.GetByValue(tagDto.Value);
            alreadyExistingTags.Add(tag);
        }

        
        var newTagsInDb = new List<Tag>();
        foreach (var tagDto in newTags)
        {
            var newTag = new Tag(tagDto.Label, tagDto.Value, tagDto.Color);
            var createdTagId = await tagRepository.CreateIfNotExists(newTag);
            var createdTag = await tagRepository.Get(createdTagId);
            newTagsInDb.Add(newTag);
        }
        
        return newTagsInDb.Concat(alreadyExistingTags).ToList();
    }
}
