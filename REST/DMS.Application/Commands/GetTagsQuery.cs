using DMS.Application.DTOs;
using DMS.Domain.Entities;
using DMS.Domain.Entities.Tag;
using DMS.Domain.IRepositories;
using MediatR;

namespace DMS.Application.Commands
{
public record GetTagsQuery() : IRequest<List<TagDto>>;

public class GetTagsQueryHandler(ITagRepository tagRepository) : IRequestHandler<GetTagsQuery, List<TagDto>>
{
    public async Task<List<TagDto>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await tagRepository.GetAll();
        // tags.Add(new Tag { Label = "project", Color = "#FF0031", Value = "project" });
        var tagDtos = tags.Select(t => t.ToDto()).ToList();
        
        return await Task.FromResult(tagDtos);
    }
}
}

