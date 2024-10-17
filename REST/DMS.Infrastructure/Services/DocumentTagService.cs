using DMS.Application.DTOs;
using DMS.Application.Interfaces;
using DMS.Domain.Entities;
using DMS.Domain.Entities.Tag;
using DMS.Domain.IRepositories;

namespace DMS.Infrastructure.Services;

public class DocumentTagService(ITagRepository tagRepository, IDocumentTagRepository documentTagRepository) : IDocumentTagService
{
    private readonly ITagRepository _tagRepository = tagRepository;
    private readonly IDocumentTagRepository _documentTagRepository = documentTagRepository;

    public async Task<List<DocumentTag>> CreateOrGetDocumentTagsFromTagsDtos(List<TagDto> tagDtos, IUnitOfWork unitOfWork)
    {
        await unitOfWork.BeginTransactionAsync();

        IEnumerable<Tag>? tagsInDb = await tagRepository.GetAll();
        var existingTagValues = new HashSet<string>(tagsInDb.Select(dbTag => dbTag.Value));

        var newTags = tagDtos
            .Where(requestTag => !existingTagValues.Contains(requestTag.Value))
            .ToList();

        var alreadyExistingTagDtos = tagDtos
            .Where(requestTag => existingTagValues.Contains(requestTag.Value));

        var alreadyExistingTags = await Task.WhenAll(
            alreadyExistingTagDtos.Select(t =>
                    tagRepository.Get(t.Id))
                .ToList());

        var newTagsInDb =
            await Task.WhenAll(
                newTags.Select(t =>
                    unitOfWork.TagRepository.Create(new Tag(t.Label, t.Value, t.Color))));

        var allTagsAssociatedWithDocument = newTagsInDb.Concat(alreadyExistingTags);
        
    }
}