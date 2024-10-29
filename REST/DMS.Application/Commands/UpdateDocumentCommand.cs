using System.Text.Json;
using AutoMapper;
using DMS.Application.DTOs;
using DMS.Application.Interfaces;
using DMS.Domain.DomainEvents;
using DMS.Domain.Entities;
using DMS.Domain.Entities.Tag;
using DMS.Domain.IRepositories;
using DMS.Domain.Services;
using DMS.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DMS.Application.Commands
{
    public record UpdateDocumentCommand(UpdateDocumentDto Document) : IRequest<DmsDocumentDto>;

    public class UpdateDocumentCommandHandler(
        IUnitOfWork unitOfWork,
        IDmsDocumentRepository dmsDocumentRepository,
        IDocumentTagFactory documentTagFactory,
        IMapper autoMapper,
        ILogger<UpdateDocumentCommandHandler> logger)
        : IRequestHandler<UpdateDocumentCommand, DmsDocumentDto>
    {
        public async Task<DmsDocumentDto> Handle(UpdateDocumentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync();

                var document = await dmsDocumentRepository.Get(request.Document.Id);


                var tagsAssociatedWithDocument = await documentTagFactory.CreateOrGetTagsFromTagDtos(request.Document.Tags);

                logger.LogInformation("Tags associated with document: {0}", JsonSerializer.Serialize(tagsAssociatedWithDocument));

                var newTags = tagsAssociatedWithDocument
                    .Where(tag => !document.Tags.Any(existingTag => existingTag.Tag.Id == tag.Id))
                    .ToList();

                logger.LogInformation("New tags: {0}", JsonSerializer.Serialize(newTags));
                List<DocumentTag> documentTags = new List<DocumentTag>();

                newTags.ForEach(tag => documentTags.Add(DocumentTag.Create(tag, document!)));

                documentTags.Concat(document.Tags);
                logger.LogInformation("Document tags: {0}", JsonSerializer.Serialize(documentTags));
                // var documentTags = await Task.WhenAll(
                //     tagsAssociatedWithDocument.Select(t =>
                //         unitOfWork.DocumentTagRepository.Create(
                //             DocumentTag.Create(t, document))));

                // var updatedDocumentTags = documentTags.ToList();

                document
                    .UpdateTitle(request.Document.Title)
                    .UpdateTags(documentTags);
                await unitOfWork.DmsDocumentRepository.UpdateAsync(document);

                document.AddDomainEvent(new DocumentUpdatedDomainEvent(document));


                await unitOfWork.CommitAsync();


                return autoMapper.Map<DmsDocumentDto>(document);
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}