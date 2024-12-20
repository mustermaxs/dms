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
using Microsoft.Extensions.Logging;

namespace DMS.Application.Commands
{
    public record UploadDocumentCommand(string Title, string Content, List<TagDto> Tags, string FileType) : IRequest<DmsDocumentDto>;

    public class UploadDocumentCommandHandler : IRequestHandler<UploadDocumentCommand, DmsDocumentDto>
    {
        private readonly FileHelper _fileHelper;
        private readonly IValidator<DmsDocument> _documentValidator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDocumentTagFactory _documentTagFactory;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger<UploadDocumentCommandHandler> _logger;

        public UploadDocumentCommandHandler(
            FileHelper fileHelper,
            IValidator<DmsDocument> documentValidator,
            IUnitOfWork unitOfWork,
            IDocumentTagFactory documentTagFactory,
            IMediator mediator,
            IMapper mapper,
            ILogger<UploadDocumentCommandHandler> logger)
        {
            _fileHelper = fileHelper;
            _documentValidator = documentValidator;
            _unitOfWork = unitOfWork;
            _documentTagFactory = documentTagFactory;
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<DmsDocumentDto> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                
                var document =  DmsDocument.Create(
                    request.Title,
                    DateTime.UtcNow,
                    null,
                    new List<DocumentTag>(),
                    request.FileType,
                    ProcessingStatus.NotStarted);

                var tagsAssociatedWithDocument = await _documentTagFactory.CreateOrGetTagsFromTagDtos(request.Tags);
                tagsAssociatedWithDocument.ForEach(tag => document.AddTag(tag));
                await _unitOfWork.DmsDocumentRepository.Create(document);
                document.AddDomainEvent(new DocumentCreatedDomainEvent(document, request.Content));
                await _unitOfWork.CommitAsync();
                
                return _mapper.Map<DmsDocumentDto>(document);
            }
            catch (Exception e)
            {
                await _unitOfWork.RollbackAsync();
                await _mediator.Publish(new FailedToCreateeDocumentIntegrationEvent(request)); 
                throw new UploadDocumentException($"Failed to upload document.");
            }
        }
    }
}