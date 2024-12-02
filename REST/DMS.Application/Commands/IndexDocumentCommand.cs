using DMS.Application.Interfaces;
using Microsoft.Extensions.Logging;

using MediatR;

namespace DMS.Application.Commands
{
    public record IndexDocumentCommand(Guid Id) : IRequest, IRequest<Unit>;
    
    public class IndexDocumentCommandHandler : IRequestHandler<IndexDocumentCommand>
    {
        private readonly IFileStorage _fileStorage;
        private readonly ILogger<IndexDocumentCommandHandler> _logger;

        public IndexDocumentCommandHandler(IFileStorage fileStorage, ILogger<IndexDocumentCommandHandler> logger)
        {
            _fileStorage = fileStorage;
            _logger = logger;
        }

        public async Task<Unit> Handle(IndexDocumentCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
