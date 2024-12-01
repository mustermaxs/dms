using DMS.Domain.Entities.Tag;
using MediatR;

namespace DMS.Application.Queries
{
public record FindTagsByPrefixQuery(string Prefix) : IRequest<List<Tag>>;
    
public class FindTagsByPrefixQueryHandler : IRequestHandler<FindTagsByPrefixQuery, List<Tag>>
{
    public Task<List<Tag>> Handle(FindTagsByPrefixQuery request, CancellationToken cancellationToken)
    {
        var tags = new List<Tag>
        {
            Tag.Create("project", "project", "#F7839"),
            Tag.Create("project", "project", "#F7839")
        };
        
        return Task.FromResult(tags);
    }
}
}

