using AutoMapper;
using DMS.Application.DTOs;
using DMS.Application.Interfaces;
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
        
        var documents = await Task.WhenAll(
            searchResults.Select(async result => await documentRepository.Get(result.Id))
        );
        
        return mapper.Map<List<DocumentSearchResultDto>>(documents);

        // return documents
        //     .Where(doc => doc != null)
        //     .Select(doc => mapper.Map<DocumentSearchResultDto>(doc))
        //     .ToList();
    }
}

}