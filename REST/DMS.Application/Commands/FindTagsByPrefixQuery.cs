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
            new Tag { Label = "contract", Color = "#FF0000", Value = "contract" },
            new Tag { Label = "project", Color = "#FF0031", Value = "project" }
        };
        
        return Task.FromResult(tags);
    }
}
}

