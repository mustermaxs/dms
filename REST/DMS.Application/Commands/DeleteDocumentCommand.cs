using DMS.Application.Interfaces;
using DMS.Domain.Entities;
using DMS.Domain.Entities.Tag;
using DMS.Domain.ValueObjects;
using MediatR;

namespace DMS.Application.Commands
{
    public record DeleteDocumentCommand(Guid Id) : IRequest, IRequest<Unit>;
    
    public class DeleteDocumentCommandHandler(
        IFileStorage fileStorage,
        IUnitOfWork unitOfWork) : IRequestHandler<DeleteDocumentCommand>
    {
        public async Task<Unit> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync();
                await unitOfWork.DmsDocumentRepository.DeleteById(request.Id);
                await fileStorage.DeleteFileAsync(request.Id);
                
                await unitOfWork.CommitAsync();
                return Unit.Value;
            }
            catch (Exception e)
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
