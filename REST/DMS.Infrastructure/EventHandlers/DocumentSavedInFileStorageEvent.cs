using DMS.Domain.Entities.DomainEvents;
using DMS.Infrastructure.Services;
using MediatR;

namespace DMS.Infrastructure.EventHandlers
{
    public record DocumentSavedInFileStorageEvent() : IRequest<Unit>;
    
    public class DocumentSavedInFileStorageEventHandler : IRequestHandler<DocumentSavedInFileStorageEvent, Unit>
    {
        public async Task<Unit> Handle(DocumentSavedInFileStorageEvent request, CancellationToken cancellationToken)
        {
            Console.WriteLine("DocumentSavedInFileStorageEventHandler");
            return await Task.FromResult(Unit.Value);
        }
    }
}