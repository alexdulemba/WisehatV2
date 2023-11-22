using MediatR;
using Wisehat.Domain.Entities;
using Wisehat.Domain.Infrastructure;

namespace Wisehat.Domain.Queries.WebProjects;

public static class GetWebProjectsByProfileId
{
  public record Query(string ProfileId) : IRequest<IEnumerable<WebProject>>;

  public class Handler(IDatabaseService database) 
    : IRequestHandler<Query, IEnumerable<WebProject>>
  {
    public Task<IEnumerable<WebProject>> Handle(Query request, CancellationToken token)
    {
      return database.GetWebProjectsByProfileIdAsync(request.ProfileId, token);
    }
  }
}
