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

    public class UpdateDocumentCommandHandler : IRequestHandler<UpdateDocumentCommand, DmsDocumentDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDmsDocumentRepository _dmsDocumentRepository;
        private readonly IDocumentTagFactory _documentTagFactory;
        private readonly IMapper _autoMapper;
        private readonly ILogger<UpdateDocumentCommandHandler> _logger;
        private readonly ISearchService _searchService;

        public UpdateDocumentCommandHandler(
            IUnitOfWork unitOfWork,
            IDmsDocumentRepository dmsDocumentRepository,
            IDocumentTagFactory documentTagFactory,
            IMapper autoMapper,
            ILogger<UpdateDocumentCommandHandler> logger,
            ISearchService searchService)
        {   
            _unitOfWork = unitOfWork;
            _dmsDocumentRepository = dmsDocumentRepository;
            _documentTagFactory = documentTagFactory;
            _autoMapper = autoMapper;
            _logger = logger;
            _searchService = searchService;
        }

        public async Task<DmsDocumentDto> Handle(UpdateDocumentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Updating document {request.Document.Id}");

                await _unitOfWork.BeginTransactionAsync();

                var document = await _dmsDocumentRepository.Get(request.Document.Id);
                var tagsAssociatedWithDocument = await _documentTagFactory.CreateOrGetTagsFromTagDtos(request.Document.Tags);
                await _unitOfWork.DocumentTagRepository.DeleteAllByDocumentId(document.Id);

                document
                    .UpdateTitle(request.Document.Title)
                    .UpdateTags(tagsAssociatedWithDocument);
                await _unitOfWork.DmsDocumentRepository.UpdateAsync(document);

                await _searchService.UpdateDocumentAsync(document);

                document.AddDomainEvent(new DocumentUpdatedDomainEvent(document));
                await _unitOfWork.CommitAsync();

                return _autoMapper.Map<DmsDocumentDto>(document);
            }
            catch (Exception e)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError($"Failed to update document {request.Document.Id}");
                _logger.LogError(e.Message);
                throw;
            }
        }
    }
}