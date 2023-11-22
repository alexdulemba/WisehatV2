using MediatR;
using Wisehat.Domain.Infrastructure;

namespace Wisehat.Domain.Commands.WebProjects;

public static class DeleteWebProject
{
  public record Command(Guid ProjectId) : IRequest<bool>;

  public class Handler(IDatabaseService database) : IRequestHandler<Command, bool>
  {
    public Task<bool> Handle(Command request, CancellationToken token)
    {
      return database.DeleteWebProjectAsync(request.ProjectId, token);
    }
  }
}
