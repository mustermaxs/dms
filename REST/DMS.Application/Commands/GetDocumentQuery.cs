using DMS.Application.DTOs;
using DMS.Domain.Entities;
using DMS.Domain.Entities.Tag;
using DMS.Domain.ValueObjects;
using MediatR;

namespace DMS.Application.Commands
{
    public record GetDocumentQuery(Guid Id) : IRequest, IRequest<DmsDocumentDto>;

    public class GetDocumentQueryHandler : IRequestHandler<GetDocumentQuery, DmsDocumentDto>
    {
        public async Task<DmsDocumentDto> Handle(GetDocumentQuery request, CancellationToken cancellationToken)
        {
            var document = new DmsDocumentDto
            {
                Id = Guid.NewGuid(), Title = "Document 1",
                UploadDateTime = DateTime.Now,
                ModificationDateTime = DateTime.Now,
                Status = ProcessingStatus.Finished,
                Tags = [new TagDto{ Label = "contract", Color = "#FF0000", Value = "contract" }],
                DocumentType = FileType.GetFileTypeFromExtension("blabla.pdf")
            };

            return await Task.FromResult(document);
        }
    }
}