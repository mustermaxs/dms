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
    public record UploadDocumentCommand(string Title, string Content, List<TagDto> Tags) : IRequest<DmsDocumentDto>;

    public class UploadDocumentCommandHandler(
        IDmsDocumentRepository documentRepository,
        ITagRepository tagRepository,
        IDocumentTagRepository documentTagRepository,
        IFileStorage fileStorage,
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
                // TODO Refactor the creation of tags/documentTags to a separate service
                await unitOfWork.BeginTransactionAsync();
                
                var document =  DmsDocument.Create(
                    request.Title,
                    DateTime.UtcNow,
                    null,
                    new List<DocumentTag>(),
                    new FileType(request.Title),
                    ProcessingStatus.NotStarted);
                
                var documentIsValid = await documentValidator.ValidateAsync(document);

                if (!documentIsValid.IsValid)
                {
                    throw new ValidationException(documentIsValid.Errors);
                }

                var tagsAssociatedWithDocument = await documentTagFactory.CreateOrGetTagsFromTagDtos(request.Tags);
                
                tagsAssociatedWithDocument.ForEach(tag => document.AddTag(tag));
                // TODO Put conversion from Base64 to FileStream in a separate service
                // or make the client send it as stream in JSON object if possible
                // await fileStorage.SaveFileAsync(document.Id, new MemoryStream(Convert.FromBase64String(request.Content)));
                
                await unitOfWork.DmsDocumentRepository.Create(document);
                document.AddDomainEvent(new DocumentUploadedToDbDomainEvent(document, request.Content));
                // TODO fileStorage.Save()...
                //          in fileStorage: dispatch Integration Event when done
                // in Integration EventHandler use MessageBroker (RabbitMQ) to inform OCR Worker to process
                // ...
                await unitOfWork.CommitAsync();
                
                return mapper.Map<DmsDocumentDto>(document);
            }
            catch (Exception e)
            {
                await unitOfWork.RollbackAsync();
                Console.WriteLine(e.Message);
                throw new UploadDocumentException($"Failed to upload document.");
            }
        }
    }
}