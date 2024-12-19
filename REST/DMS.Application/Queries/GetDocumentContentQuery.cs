using AutoMapper;
using DMS.Application.DTOs;
using DMS.Domain.IRepositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DMS.Application.Commands
{
    public record GetDocumentContentQuery(Guid Id) : IRequest<DocumentContentDto>;

    public class GetDocumentContentQueryHandler : IRequestHandler<GetDocumentContentQuery, DocumentContentDto>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<GetDocumentContentQueryHandler> _logger;
        private readonly IDmsDocumentRepository _documentRepository;


        public GetDocumentContentQueryHandler(IMapper mapper, ILogger<GetDocumentContentQueryHandler> logger, IDmsDocumentRepository documentRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _documentRepository = documentRepository;
        }

        public async Task<DocumentContentDto> Handle(GetDocumentContentQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting document content for document ID: {DocumentId}", request.Id);
            var document = await _documentRepository.Get(request.Id);
            return _mapper.Map<DocumentContentDto>(document);
        }
    }
}
