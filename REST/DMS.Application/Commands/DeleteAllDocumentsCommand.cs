using DMS.Application.Interfaces;
using DMS.Domain.IRepositories;
using MediatR;

namespace DMS.Application.Commands
{
    public record DeleteAllDocumentsCommand() : IRequest<Unit>;
    
    public class DeleteAllDocumentsCommandHandler(
        IUnitOfWork unitOfWork,
        IFileStorage fileStorage) : IRequestHandler<DeleteAllDocumentsCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteAllDocumentsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync();
                await unitOfWork.DmsDocumentRepository.DeleteAllAsync();
                await fileStorage.DeleteAllFilesAsync();
                await unitOfWork.CommitAsync();
                return Unit.Value;
            }
            catch (Exception e)
            {
                await unitOfWork.RollbackAsync();
                Console.WriteLine(e);
                throw;
            }
        }
    }
}

