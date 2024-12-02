using AutoMapper;
using DMS.Application.DTOs;
using DMS.Domain.DomainEvents;
using DMS.Domain.Entities;
using DMS.Domain.Entities.Tag;
using DMS.Domain.IRepositories;
using DMS.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DMS.Application.Commands
{
    public record GetDocumentsQuery : IRequest, IRequest<List<DmsDocumentDto>>;

    public class GetDocumentsQueryHandler : IRequestHandler<GetDocumentsQuery, List<DmsDocumentDto>>
    {
        private readonly IDmsDocumentRepository _documentRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetDocumentsQueryHandler> _logger;

        public GetDocumentsQueryHandler(IDmsDocumentRepository documentRepository, IMapper mapper, ILogger<GetDocumentsQueryHandler> logger)
        {
            _documentRepository = documentRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<DmsDocumentDto>> Handle(GetDocumentsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting all documents");

            var documents = await _documentRepository.GetAll();
            var documentDtos = documents.Select(d => _mapper.Map<DmsDocumentDto>(d)).ToList();
            return Task.FromResult(documentDtos).Result;
        }
    }
}