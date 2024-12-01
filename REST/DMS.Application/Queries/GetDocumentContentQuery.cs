using AutoMapper;
using DMS.Application.DTOs;
using DMS.Domain.IRepositories;
using MediatR;

namespace DMS.Application.Commands
{
    public record GetDocumentContentQuery(Guid Id) : IRequest<DocumentContentDto>;

    public class GetDocumentContentQueryHandler(
        IMapper mapper,
        IDmsDocumentRepository documentRepository) : IRequestHandler<GetDocumentContentQuery, DocumentContentDto>
    {
        public async Task<DocumentContentDto> Handle(GetDocumentContentQuery request,
            CancellationToken cancellationToken)
        {
            var document = await documentRepository.Get(request.Id);
            return mapper.Map<DocumentContentDto>(document);
        }
    }
}
