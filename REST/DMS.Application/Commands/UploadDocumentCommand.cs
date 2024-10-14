
using MediatR;

namespace DMS.Application.Commands
{
    public record UploadDocumentCommand(string Title) : IRequest<object>;

    public class UploadDocumentRequestHandler : IRequestHandler<UploadDocumentCommand, object>
    {
        // public async Task<object> Handle(UrploadDocumentCommand command)
        // {
        //     // return Ok object with "Id" field (Guid)
        //     return new { Id = Guid.NewGuid() };
        // }

        public async Task<object> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
        {
            return new { Id = Guid.NewGuid(), Info = "Uploaded Document" };
        }
    }
}