


namespace DMS.Application.Commands
{
    using Domain.Entities.Tags;
    using AutoMapper;
    using DTOs;
    using Interfaces;
    using MediatR;
    
    public record CreateTagCommand(string Label, string Value) : IRequest<TagDto>;

    public class CreateTagCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper) : IRequestHandler<CreateTagCommand, TagDto>
    {
        public async Task<TagDto> Handle(CreateTagCommand request, CancellationToken cancellationToken)
        {
            await unitOfWork.BeginTransactionAsync();
            var tag = await unitOfWork.TagRepository.Create(new Tag(request.Label, request.Value, "#FF0000"));
            await unitOfWork.CommitAsync();
            return mapper.Map<TagDto>(tag);
        }
    }
}