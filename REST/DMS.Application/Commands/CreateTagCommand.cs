using DMS.Domain.Entities;
using DMS.Domain.Entities.Tag;
using DMS.Domain.IRepositories;
using MediatR;

namespace DMS.Application.Commands
{
public record CreateTagCommand(string Label, string Value) : IRequest<Tag>;
    
public class CreateTagCommandHandler(ITagRepository tagRepository) : IRequestHandler<CreateTagCommand, Tag>
{
    public async Task<Tag> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await tagRepository.Create(
            new Tag
            {
                Id = new Guid(),
                Label = "project",
                Color = "#FF0031",
                Value = "project"
            });

        return tag;
    }
}
}
