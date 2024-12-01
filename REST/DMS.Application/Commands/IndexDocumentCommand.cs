using DMS.Application.Interfaces;

using MediatR;

namespace DMS.Application.Commands
{
    public record IndexDocumentCommand(Guid Id) : IRequest, IRequest<Unit>;
    
    public class IndexDocumentCommandHandler(
        IFileStorage fileStorage,
        IUnitOfWork unitOfWork,
        IMediator mediator) : IRequestHandler<IndexDocumentCommand>
    {
        public async Task<Unit> Handle(IndexDocumentCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
