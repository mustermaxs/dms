using System.Text.Json;
using AutoMapper;
using DMS.Application.DTOs;
using DMS.Application.Interfaces;
using DMS.Domain.Entities;
using DMS.Domain.IRepositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DMS.Application.Commands 
{

    public record SearchDocumentsQuery(string Query) : IRequest<List<DocumentSearchResultDto>>;

    public class SearchDocumentsQueryHandler : IRequestHandler<SearchDocumentsQuery, List<DocumentSearchResultDto>>
    {
        private readonly ISearchService _searchService;
        private readonly IDmsDocumentRepository _documentRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<SearchDocumentsQueryHandler> _logger;

        public SearchDocumentsQueryHandler(ISearchService searchService, IDmsDocumentRepository documentRepository, IMapper mapper, ILogger<SearchDocumentsQueryHandler> logger)
        {
            _searchService = searchService;
            _documentRepository = documentRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<DocumentSearchResultDto>> Handle(SearchDocumentsQuery request, CancellationToken cancellationToken)
        {
            var searchResults = await _searchService.SearchAsync(request.Query);

            _logger.LogInformation("Search results: {SearchResults}", JsonSerializer.Serialize(searchResults));

            var documents = new List<DmsDocument>();
            foreach (var result in searchResults)
            {
                var document = await _documentRepository.Get(result.Id);
                if (document != null)
                {
                    documents.Add(document);
                }
            }   
            
            return _mapper.Map<List<DocumentSearchResultDto>>(documents);
        }
    }

}