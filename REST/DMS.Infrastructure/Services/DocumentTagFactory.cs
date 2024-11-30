using DMS.Application.DTOs;
using DMS.Application.Interfaces;
using DMS.Domain.Entities.Tags;
using DMS.Domain.IRepositories;
using DMS.Domain.Services;
using DMS.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Services;

public class DocumentTagFactory(
    ITagRepository tagRepository,
    DmsDbContext dbContext) : IDocumentTagFactory
{
    public async Task<List<Tag>> CreateOrGetTagsFromTagDtos(List<Tag> tags)
    {
        // Get all tags from the database
        IEnumerable<Tag>? tagsInDb = await tagRepository.GetAll();
        var existingTagValues = new HashSet<string>(tagsInDb.Select(dbTag => dbTag.Value));

        // Separate new tags from existing tags
        var newTags = tags.Where(requestTag =>
            !existingTagValues.Contains(requestTag.Value));

        var alreadyExistingTagDtos = tagsInDb.Where(dbTag =>
            tags.Any(requestTag => requestTag.Value == dbTag.Value));

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
            // var createdTag = await tagRepository.Create(newTag);  // Awaiting each call
            newTagsInDb.Add(newTag);
        }

        // Return the combined result of new and existing tags
        return newTagsInDb.Concat(alreadyExistingTags).ToList();
    }

    public async Task<List<TagModel?>> CreateNewTagsOrGetExisting(List<Tag?> tags)
    {
        if (tags == null)
            return null;
        
        var dbSet = dbContext.Set<TagModel>();

        var tagValues = tags.Select(tag => tag.Value).ToList();
        var existingTags = await dbSet
            .Where(tag => tagValues.Contains(tag.Value))
            .ToListAsync();

        List<TagModel?> newTags = tags
            .Where(tag => !existingTags.Any(existingTag => existingTag.Value == tag.Value))
            .Select(tag => new TagModel(Guid.NewGuid(), tag.Label, tag.Value, tag.Color))
            .ToList();

        if (newTags.Any())
        {
            await dbSet.AddRangeAsync(newTags);
        }

        return newTags.Concat(existingTags).ToList();
    }

}