using DMS.Application.DTOs;
using DMS.Domain.Entities;
using DMS.Domain.IRepositories;

namespace DMS.Application.Interfaces;

public interface IDocumentTagService
{
    Task<List<DocumentTag>> CreateOrGetDocumentTagsFromTagsDtos(List<TagDto> tagDtos, IUnitOfWork unitOfWork)
}