using DMS.Application.Interfaces;
using DMS.Domain.DomainEvents;
using DMS.Domain.IRepositories;
using DMS.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DMS.Application.Commands
{
    public record DeleteDocumentCommand(Guid Id) : IRequest, IRequest<Unit>;
    
    public class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand, Unit>
    {
        private readonly IFileStorage _fileStorage;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly IDmsDocumentRepository _documentRepository;
        private readonly ILogger<DeleteDocumentCommandHandler> _logger;

        public DeleteDocumentCommandHandler(IFileStorage fileStorage, IUnitOfWork unitOfWork, IMediator mediator, IDmsDocumentRepository documentRepository, ILogger<DeleteDocumentCommandHandler> logger)
        {
            _fileStorage = fileStorage;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _documentRepository = documentRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Deleting document {request.Id}");

                var document = await _documentRepository.Get(request.Id);
                
                if (document is null)
                    throw new Exception($"Document {request.Id} not found");

                if (document.Status < ProcessingStatus.Finished)
                {
                    _logger.LogInformation($"Document {request.Id} cannot be deleted. Is still processing.");
                    return Unit.Value;
                }
                await _unitOfWork.BeginTransactionAsync();
                await _unitOfWork.DmsDocumentRepository.DeleteById(request.Id);
                await _fileStorage.DeleteFileAsync(request.Id);
                document!.AddDomainEvent(new DeletedDocumentDomainEvent(document));
                await _unitOfWork.CommitAsync();
                return Unit.Value;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to delete document {request.Id}");
                throw;
            }
        }
    }
}