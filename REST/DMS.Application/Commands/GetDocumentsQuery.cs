using DMS.Application.DTOs;
using DMS.Domain.Entities;
using DMS.Domain.Entities.Tag;
using DMS.Domain.ValueObjects;
using MediatR;

namespace DMS.Application.Commands
{
    public record GetDocumentsQuery : IRequest, IRequest<List<DmsDocumentDto>>
    {};
    
    public class GetDocumentsQueryHandler : IRequestHandler<GetDocumentsQuery, List<DmsDocumentDto>>
    {
        public Task<List<DmsDocumentDto>> Handle(GetDocumentsQuery request, CancellationToken cancellationToken)
        {
            var documents = new List<DmsDocumentDto>();
            documents.Add(new DmsDocumentDto
            {
                Id = Guid.NewGuid(), Title = "Document 1.pdf",
                UploadDateTime = DateTime.Now,
                ModificationDateTime = DateTime.Now,
                Status = ProcessingStatus.Finished,
                Tags = [new TagDto{ Label = "contract", Color = "#FF0000", Value = "contract" }],
                DocumentType = FileType.GetFileTypeFromExtension("blabla.pdf")
            });
            
            return Task.FromResult(documents);
        }
    }
}