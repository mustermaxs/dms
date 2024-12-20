using DMS.Domain.Entities;
using DMS.Domain.Entities.Tag;
using MediatR;

namespace DMS.Application.Commands
{
public record FindTagsByPrefixQuery(string Prefix) : IRequest<List<Tag>>;
    
public class FindTagsByPrefixQueryHandler : IRequestHandler<FindTagsByPrefixQuery, List<Tag>>
{
    public Task<List<Tag>> Handle(FindTagsByPrefixQuery request, CancellationToken cancellationToken)
    {
        var tags = new List<Tag>
        {
            new Tag("project", "project", "#F7839"),
            new Tag("project", "project", "#F7839")
        };
        
        return Task.FromResult(tags);
    }
}
}

