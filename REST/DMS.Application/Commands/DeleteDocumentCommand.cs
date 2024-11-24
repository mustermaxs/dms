


namespace DMS.Application.Commands
{
    using Interfaces;
    using Domain.DomainEvents;
    using Domain.IRepositories;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using Domain.Entities.DmsDocument.ValueObjects;
    
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
                await unitOfWork.DmsDocumentRepository.DeleteById(request.Id);
                await fileStorage.DeleteFileAsync(request.Id);
                document!.AddDomainEvent(new DeletedDocumentDomainEvent(document));
                await unitOfWork.CommitAsync();
                return Unit.Value;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}