using DMS.Application.DTOs;
using DMS.Domain.Entities;
using MediatR;

namespace DMS.Application.IntegrationEvents
{
    public record DocumentSavedInFileStorageIntegrationEvent(DmsDocument document) : IRequest<Unit>;
    
    public class DocumentSavedInFileStorageIntegrationEventHandler(
        ) : IRequestHandler<DocumentSavedInFileStorageIntegrationEvent, Unit>
    {
        public async Task<Unit> Handle(DocumentSavedInFileStorageIntegrationEvent request, CancellationToken cancellationToken)
        {
            Console.WriteLine("DocumentSavedInFileStorageEventHandler");
            return await Task.FromResult(Unit.Value);
        }
    }
}