using System.Collections;
using System.Reflection;
using DMS.Application.DTOs;
using DMS.Application.Interfaces;
using DMS.Domain.Entities;
using DMS.Domain.Entities.Tag;
using DMS.Domain.IRepositories;
using DMS.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace DMS.Application.Commands
{
    public record UploadDocumentCommand(string Title, string Content, List<TagDto> Tags) : IRequest<Unit>;

    public class UploadDocumentRequestHandler(
        IDmsDocumentRepository documentRepository,
        ITagRepository tagRepository,
        IDocumentTagRepository documentTagRepository,
        IFileStorage fileStorage,
        IValidator<DmsDocument> documentValidator,
        IUnitOfWork unitOfWork,
        IDocumentTagService documentTagService
        ) : IRequestHandler<UploadDocumentCommand, Unit>
    {
        public async Task<Unit> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // TODO Refactor the creation of tags/documentTags to a separate service
                await unitOfWork.BeginTransactionAsync();
                var tagsAssociatedWithDocument = await documentTagService.CreateOrGetDocumentTagsFromTagsDtos(request.Tags, unitOfWork);
                
                var document = new DmsDocument(
                    Guid.NewGuid(),
                    request.Title,
                    request.Content,
                    DateTime.Now,
                    DateTime.Now,
                    null,
                    new List<DocumentTag>(),
                    FileType.GetFileTypeFromExtension(request.Title),
                    ProcessingStatus.NotStarted);

                var isValidDocument = (await documentValidator.ValidateAsync(document)).IsValid;


                await unitOfWork.DmsDocumentRepository.Create(document);
                await Task.WhenAll(tagsAssociatedWithDocument.Select(t =>
                    unitOfWork.DocumentTagRepository.Create(
                        new DocumentTag
                        {
                            Document = document,
                            TagId = t.Id,
                            DocumentId = document.Id,
                            Tag = t
                        })));
                
                await unitOfWork.CommitAsync();
                // TODO Put conversion from Base64 to FileStream in a separate service
                // or make the client send it as stream in JSON object if possible
                await fileStorage.SaveFileAsync(document.Id, new MemoryStream(Convert.FromBase64String(request.Content)));
                return Unit.Value;
            }
            catch (Exception)
            {
                await unitOfWork.RollbackAsync();
                return Unit.Value;
            }
        }
    }
}