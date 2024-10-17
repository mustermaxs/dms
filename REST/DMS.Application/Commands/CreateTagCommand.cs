using DMS.Application.DTOs;
using DMS.Domain.Entities;
using DMS.Domain.Entities.Tag;
using DMS.Domain.IRepositories;
using MediatR;

namespace DMS.Application.Commands
{
public record CreateTagCommand(string Label, string Value) : IRequest<TagDto>;
    
public class CreateTagCommandHandler(ITagRepository tagRepository) : IRequestHandler<CreateTagCommand, TagDto>
{
    public async Task<TagDto> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await tagRepository.Create(new Tag("project", "project", "#F7839"));;
        
        return tag.ToDto();
    }
}
}
