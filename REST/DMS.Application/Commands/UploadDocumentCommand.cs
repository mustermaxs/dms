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
        IUnitOfWork unitOfWork) : IRequestHandler<UploadDocumentCommand, Unit>
    {
        public async Task<Unit> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // TODO Refactor the creation of tags/documentTags to a separate service
                await unitOfWork.BeginTransactionAsync();

                IEnumerable<Tag>? tagsInDb = await tagRepository.GetAll();
                var existingTagValues = new HashSet<string>(tagsInDb.Select(dbTag => dbTag.Value));

                var newTags = request.Tags
                    .Where(requestTag => !existingTagValues.Contains(requestTag.Value))
                    .ToList();

                var alreadyExistingTagDtos = request.Tags
                    .Where(requestTag => existingTagValues.Contains(requestTag.Value));

                var alreadyExistingTags = await Task.WhenAll(
                    alreadyExistingTagDtos.Select(t =>
                            tagRepository.Get(t.Id))
                        .ToList());

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

                var newTagsInDb =
                    await Task.WhenAll(
                        newTags.Select(t =>
                            unitOfWork.TagRepository.Create(new Tag(t.Label, t.Value, t.Color))));

                var allTagsAssociatedWithDocument = newTagsInDb.Concat(alreadyExistingTags);
                await unitOfWork.DmsDocumentRepository.Create(document);
                await Task.WhenAll(allTagsAssociatedWithDocument.Select(t =>
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