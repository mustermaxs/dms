using DMS.Application.Interfaces;
using DMS.Domain.DomainEvents;
using DMS.Domain.IRepositories;
using DMS.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DMS.Application.Commands
{
    public record DeleteDocumentCommand(Guid Id) : IRequest, IRequest<Unit>;
    
    public class DeleteDocumentCommandHandler(
        IFileStorage fileStorage,
        IUnitOfWork unitOfWork,
        IMediator mediator,
        IDmsDocumentRepository documentRepository,
        ILogger<DeleteDocumentCommandHandler> logger) : IRequestHandler<DeleteDocumentCommand>
    {
        public async Task<Unit> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var document = await documentRepository.Get(request.Id);
                
                if (document is null)
                    throw new Exception($"Document {request.Id} not found");

                if (document.Status < ProcessingStatus.Finished)
                {
                    logger.LogInformation($"Document {request.Id} cannot be deleted. Is still processing.");
                    return Unit.Value;
                }
                await unitOfWork.BeginTransactionAsync();
                document!.AddDomainEvent(new DeletedDocumentDomainEvent(document));
                await unitOfWork.DmsDocumentRepository.Delete(document);
                await unitOfWork.CommitAsync();
                return Unit.Value;
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                throw;
            }
        }
    }
}