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
                await unitOfWork.DocumentTagRepository.DeleteAllByDocumentId(document.Id);

                document
                    .UpdateTitle(request.Document.Title)
                    .UpdateTags(tagsAssociatedWithDocument);
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