using System.Text.Json;
using AutoMapper;
using DMS.Application.DTOs;
using DMS.Application.Interfaces;
using DMS.Domain.Entities;
using DMS.Domain.IRepositories;
using MediatR;

namespace DMS.Application.Commands 
{

    public record SearchDocumentsQuery(string Query) : IRequest<List<DocumentSearchResultDto>>;

    public class SearchDocumentsQueryHandler(
        ISearchService searchService,
        IDmsDocumentRepository documentRepository,
        IMapper mapper
    ) : IRequestHandler<SearchDocumentsQuery, List<DocumentSearchResultDto>>
    {

        public async Task<List<DocumentSearchResultDto>> Handle(SearchDocumentsQuery request, CancellationToken cancellationToken)
        {
            var searchResults = await searchService.SearchAsync(request.Query);

            var documents = new List<DmsDocument>();
            foreach (var result in searchResults)
            {
                var document = await documentRepository.Get(result.Id);
                if (document != null)
                {
                    documents.Add(document);
                }
            }   
            
            return mapper.Map<List<DocumentSearchResultDto>>(documents);
        }
    }

}