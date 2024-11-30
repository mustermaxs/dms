using DMS.Application.DTOs;
using DMS.Application.Interfaces;
using DMS.Domain.Entities.Tags;
using DMS.Domain.IRepositories;
using DMS.Infrastructure.Entities;

namespace DMS.Domain.Services;

public interface IDocumentTagFactory
{
    public Task<List<Tag>> CreateOrGetTagsFromTagDtos(List<Tag> tags);
    public Task<List<TagModel?>> CreateNewTagsOrGetExisting(List<Tag?> tags);
}