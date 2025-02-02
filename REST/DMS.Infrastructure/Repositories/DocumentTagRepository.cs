using DMS.Application;
using DMS.Domain;
using DMS.Domain.Entities;
using DMS.Domain.IRepositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Repositories;

public class DocumentTagRepository(DmsDbContext dbContext, IValidator<DocumentTag> validator)
    : BaseRepository<DocumentTag>(dbContext, validator),
        IDocumentTagRepository
{
    public async Task<DocumentTag> CreateOrGetIfExists(DocumentTag documentTag)
    {
        await validator.ValidateAndThrowAsync(documentTag);
        var existingTag = await Get(documentTag.TagId);
        return existingTag ?? await Create(documentTag);
    }

    public async Task<List<DocumentTag>> GetAllByDocumentId(Guid tagId)
    {
        return await DbSet.Where(dt => dt.DocumentId == tagId).ToListAsync();
    }

    public async Task DeleteAllByDocumentId(Guid documentId)
    {
        var documentTags = await GetAllByDocumentId(documentId);
        foreach (var documentTag in documentTags)
        {
            await Delete(documentTag);
        }
    }
}