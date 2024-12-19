using System.Text.Json;
using AutoMapper;
using DMS.Application.DTOs;
using DMS.Domain;
using DMS.Domain.DomainEvents;
using DMS.Domain.Entities;
using DMS.Domain.Entities.Tag;
using DMS.Domain.IRepositories;
using DMS.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DMS.Application.Commands
{
    public record GetDocumentQuery(Guid Id) : IRequest<DmsDocumentDto>;

    public class GetDocumentQueryHandler : IRequestHandler<GetDocumentQuery, DmsDocumentDto>
    {
        private readonly IDmsDocumentRepository _documentRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetDocumentQueryHandler> _logger;

        public GetDocumentQueryHandler(IDmsDocumentRepository documentRepository, IMapper mapper, ILogger<GetDocumentQueryHandler> logger)
        {
            _documentRepository = documentRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<DmsDocumentDto> Handle(GetDocumentQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting document with ID: {DocumentId}", request.Id);
            
            var document = await _documentRepository.Get(request.Id);
            
            if (document == null)
            {
                _logger.LogWarning("Document not found with ID: {DocumentId}", request.Id);
                throw new Exception($"Document with ID {request.Id} not found");
            }

            return _mapper.Map<DmsDocumentDto>(document);
        }
    }
}