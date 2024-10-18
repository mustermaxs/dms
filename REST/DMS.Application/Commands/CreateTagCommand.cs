using DMS.Application.DTOs;
using DMS.Domain.Entities;
using DMS.Domain.Entities.DomainEvents;
using DMS.Domain.Entities.Tag;
using DMS.Domain.IRepositories;
using MediatR;

namespace DMS.Application.Commands
{
    public record CreateTagCommand(string Label, string Value) : IRequest<TagDto>;

    public class CreateTagCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateTagCommand, TagDto>
    {
        public async Task<TagDto> Handle(CreateTagCommand request, CancellationToken cancellationToken)
        {
            await unitOfWork.BeginTransactionAsync();
            var tag = await unitOfWork.TagRepository.Create(new Tag("project", "project", "#F7839"));
            ;
            await unitOfWork.CommitAsync();
            return tag.ToDto();
        }
    }
}