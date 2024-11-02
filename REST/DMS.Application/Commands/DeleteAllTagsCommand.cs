using DMS.Application.Interfaces;
using MediatR;

namespace DMS.Application.Commands
{
    public record DeleteAllTagsCommand() : IRequest<Unit>;
    
    public class DeleteAllTagsCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteAllTagsCommand>
    {
        public async Task<Unit> Handle(DeleteAllTagsCommand request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(Unit.Value);
        }
    }
}

