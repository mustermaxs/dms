using AutoMapper;
using DMS.Application.DTOs;
using DMS.Domain.DomainEvents;
using DMS.Domain.Entities;
using DMS.Domain.Entities.Tag;
using DMS.Domain.IRepositories;
using DMS.Domain.ValueObjects;
using MediatR;

namespace DMS.Application.Commands
{
    public record GetDocumentsQuery : IRequest, IRequest<List<DmsDocumentDto>>
    {};
    
    public class GetDocumentsQueryHandler(
        IDmsDocumentRepository documentRepository,
        IMapper mapper,
        IMediator mediator) : IRequestHandler<GetDocumentsQuery, List<DmsDocumentDto>>
    {
        public async Task<List<DmsDocumentDto>> Handle(GetDocumentsQuery request, CancellationToken cancellationToken)
        {
            var documents = await documentRepository.GetAll();
            var documentDtos = documents.Select(d => mapper.Map<DmsDocumentDto>(d)).ToList();
            return Task.FromResult(documentDtos).Result;
        }
    }
}