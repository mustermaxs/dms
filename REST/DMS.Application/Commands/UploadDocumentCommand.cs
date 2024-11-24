using AutoMapper;
using DMS.Application.DTOs;
using DMS.Application.Exceptions;
using DMS.Application.IntegrationEvents;
using DMS.Application.Interfaces;
using DMS.Application.Services;
using DMS.Domain.DomainEvents;
using DMS.Domain.Entities.DmsDocument;
using DMS.Domain.Entities.DmsDocument.ValueObjects;
using DMS.Domain.Entities.Tags;
using DMS.Domain.Services;
using DMS.Domain.ValueObjects;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DMS.Application.Commands
{
    public record UploadDocumentCommand(string Title, string Content, List<TagDto> Tags) : IRequest<DmsDocumentDto>;

    public class UploadDocumentCommandHandler(
        FileHelper fileHelper,
        IValidator<DmsDocument> documentValidator,
        IUnitOfWork unitOfWork,
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
                    mapper.Map<List<Tag>>(request.Tags),
                    new FileType(request.Title),
                    ProcessingStatus.NotStarted);
                
                var documentIsValid = await documentValidator.ValidateAsync(document);

                if (!documentIsValid.IsValid)
                {
                    throw new ValidationException(documentIsValid.Errors);
                }
                
                await unitOfWork.DmsDocumentRepository.Create(document);
                document.AddDomainEvent(new DocumentUploadedToDbDomainEvent(document, request.Content));
                await unitOfWork.CommitAsync();
                
                return mapper.Map<DmsDocumentDto>(document);
            }
            catch (Exception e)
            {
                await unitOfWork.RollbackAsync();
                // TODO: Add integration event to notify that document upload failed
                await mediator.Publish(new FailedToCreateeDocumentIntegrationEvent(request)); 
                throw new UploadDocumentException($"Failed to upload document.");
            }
        }
    }
}