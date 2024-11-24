using AutoMapper;
using DMS.Application.DTOs;
using DMS.Application.Exceptions;
using DMS.Application.IntegrationEvents;
using DMS.Application.Interfaces;
using DMS.Application.Services;
using DMS.Domain.DomainEvents;
using DMS.Domain.Entities;
using DMS.Domain.IRepositories;
using DMS.Domain.Services;
using DMS.Domain.ValueObjects;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DMS.Application.Commands
{
    public record UploadDocumentCommand(string Title, string Content, List<TagDto> Tags, string FileType) : IRequest<DmsDocumentDto>;

    public class UploadDocumentCommandHandler(
        FileHelper fileHelper,
        IValidator<DmsDocument> documentValidator,
        IUnitOfWork unitOfWork,
        IDocumentTagFactory documentTagFactory,
        IMediator mediator,
        IMapper mapper
        ) : IRequestHandler<UploadDocumentCommand, DmsDocumentDto>
    {
        public async Task<DmsDocumentDto> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync();
                
                var document =  DmsDocument.Create(
                    request.Title,
                    DateTime.UtcNow,
                    null,
                    new List<DocumentTag>(),
                    new FileType(request.FileType),
                    ProcessingStatus.NotStarted);
                
                var documentIsValid = await documentValidator.ValidateAsync(document);

                if (!documentIsValid.IsValid)
                {
                    throw new ValidationException(documentIsValid.Errors);
                }

                var tagsAssociatedWithDocument = await documentTagFactory.CreateOrGetTagsFromTagDtos(request.Tags);
                tagsAssociatedWithDocument.ForEach(tag => document.AddTag(tag));
                await unitOfWork.DmsDocumentRepository.Create(document);
                document.AddDomainEvent(new DocumentUploadedToDbDomainEvent(document, request.Content));
                await unitOfWork.CommitAsync();
                
                return mapper.Map<DmsDocumentDto>(document);
            }
            catch (Exception e)
            {
                await unitOfWork.RollbackAsync();
                await mediator.Publish(new FailedToCreateeDocumentIntegrationEvent(request)); 
                throw new UploadDocumentException($"Failed to upload document.");
            }
        }
    }
}