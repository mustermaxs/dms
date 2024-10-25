using AutoMapper;
using DMS.Application.DTOs;
using DMS.Domain;
using DMS.Domain.Entities;
using DMS.Domain.Entities.Tag;
using DMS.Domain.IRepositories;
using DMS.Domain.ValueObjects;
using MediatR;

namespace DMS.Application.Commands
{
    public record GetDocumentQuery(Guid Id) : IDomainEvent, IRequest<DmsDocumentDto>;

    public class GetDocumentQueryHandler(
        IMediator mediator,
        IMapper mapper,
        IDmsDocumentRepository documentRepository) : IRequestHandler<GetDocumentQuery, DmsDocumentDto>
    {
        public async Task<DmsDocumentDto> Handle(GetDocumentQuery request, CancellationToken cancellationToken)
        {
            var document = await documentRepository.Get(request.Id);
            return mapper.Map<DmsDocumentDto>(document);
        }
    }
}