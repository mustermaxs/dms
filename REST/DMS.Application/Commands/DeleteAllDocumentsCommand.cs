using DMS.Application.Interfaces;
using DMS.Domain.IRepositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DMS.Application.Commands
{
    public record DeleteAllDocumentsCommand() : IRequest<Unit>;
    
    public class DeleteAllDocumentsCommandHandler : IRequestHandler<DeleteAllDocumentsCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorage _fileStorage;
        private readonly ILogger<DeleteAllDocumentsCommandHandler> _logger;
        private readonly ISearchService _searchService;

        public DeleteAllDocumentsCommandHandler(
            IUnitOfWork unitOfWork,
            IFileStorage fileStorage,
            ILogger<DeleteAllDocumentsCommandHandler> logger,
            ISearchService searchService)
        {
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
            _logger = logger;
            _searchService = searchService;
        }

        public async Task<Unit> Handle(DeleteAllDocumentsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Deleting all documents");

                await _unitOfWork.BeginTransactionAsync();
                await _unitOfWork.DmsDocumentRepository.DeleteAllAsync();
                await _fileStorage.DeleteAllFilesAsync();
                // await _searchService.DeleteAllAsync();
                await _unitOfWork.CommitAsync();
                return Unit.Value;
            }
            catch (Exception e)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(e, "Failed to delete all documents");
                throw;
            }
        }
    }
}

