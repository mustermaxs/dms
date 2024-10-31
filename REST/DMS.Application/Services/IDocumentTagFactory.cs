using DMS.Application.DTOs;
using DMS.Application.Interfaces;
using DMS.Domain.Entities.Tag;
using DMS.Domain.IRepositories;

namespace DMS.Domain.Services;

public interface IDocumentTagFactory
{
    public Task<List<Tag>> CreateOrGetTagsFromTagDtos(List<TagDto> tagDtos);
}