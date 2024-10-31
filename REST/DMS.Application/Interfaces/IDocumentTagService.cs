using DMS.Application.DTOs;
using DMS.Domain.Entities;
using DMS.Domain.Entities.Tag;
using DMS.Domain.IRepositories;

namespace DMS.Application.Interfaces;

public interface IDocumentTagService
{
    Task<List<Tag>> CreateOrGetTagsFromTagDtos(List<TagDto> tagDtos);
}