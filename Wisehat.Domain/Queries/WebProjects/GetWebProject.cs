using MediatR;
using Wisehat.Domain.Entities;
using Wisehat.Domain.Infrastructure;

namespace Wisehat.Domain.Queries.WebProjects;

public static class GetWebProject
{
  public record Query(Guid ProjectId) : IRequest<WebProject?>;

  public class Handler(IDatabaseService database) 
    : IRequestHandler<Query, WebProject?>
  {
    public Task<WebProject?> Handle(Query request, CancellationToken token)
    {
      return database.GetWebProjectAsync(request.ProjectId, token);
    }
  }
}
