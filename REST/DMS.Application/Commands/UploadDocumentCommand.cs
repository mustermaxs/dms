
using DMS.Application.DTOs;
using DMS.Domain.Entities;
using DMS.Domain.Entities.Tag;
using DMS.Domain.ValueObjects;
using MediatR;

namespace DMS.Application.Commands
{
    public record UploadDocumentCommand(string Title, byte[] Content) : IRequest<DmsDocumentDto>;

    public class UploadDocumentRequestHandler : IRequestHandler<UploadDocumentCommand, DmsDocumentDto>
    {
        public async Task<DmsDocumentDto> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
        {
            var document = new DmsDocumentDto
            {
                Id = Guid.NewGuid(), Title = "Document 1",
                UploadDateTime = DateTime.Now,
                ModificationDateTime = DateTime.Now,
                Status = ProcessingStatus.Finished,
                Tags = [new TagDto{ Label = "contract", Color = "#FF0000", Value = "contract" }],
                DocumentType = new FileType { Name = "PDF" }
            };
            
            return await Task.FromResult(document);
        }
    }
}