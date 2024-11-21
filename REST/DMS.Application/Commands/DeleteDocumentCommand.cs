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