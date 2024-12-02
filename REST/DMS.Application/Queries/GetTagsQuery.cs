using AutoMapper;
using DMS.Application.DTOs;
using DMS.Domain.IRepositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DMS.Application.Commands
{
    public record GetTagsQuery() : IRequest<List<TagDto>>;

    public class GetTagsQueryHandler: IRequestHandler<GetTagsQuery, List<TagDto>>
    {
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetTagsQueryHandler> _logger;

        public GetTagsQueryHandler(ITagRepository tagRepository, IMapper mapper, ILogger<GetTagsQueryHandler> logger)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<TagDto>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting all tags");

            var tags = await _tagRepository.GetAll();
            var tagDtos = tags.Select(t => _mapper.Map<TagDto>(t)).ToList();

            return await Task.FromResult(tagDtos);
        }
    }
}