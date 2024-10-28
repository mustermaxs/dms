using AutoMapper;
using DMS.Application.DTOs;
using DMS.Domain.Entities;
using DMS.Domain.Entities.Tag;
using DMS.Domain.IRepositories;
using MediatR;

namespace DMS.Application.Commands
{
public record GetTagsQuery() : IRequest<List<TagDto>>;

public class GetTagsQueryHandler(
    ITagRepository tagRepository,
    IMapper mapper) : IRequestHandler<GetTagsQuery, List<TagDto>>
{
    public async Task<List<TagDto>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await tagRepository.GetAll();
        var tagDtos = tags.Select(t => mapper.Map<TagDto>(t)).ToList();
        
        return await Task.FromResult(tagDtos);
    }
}
}

