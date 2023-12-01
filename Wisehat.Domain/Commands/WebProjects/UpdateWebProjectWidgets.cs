using MediatR;
using Wisehat.Domain.Entities;
using Wisehat.Domain.Infrastructure;

namespace Wisehat.Domain.Commands.WebProjects;

public static class UpdateWebProjectWidgets
{
  public record Command(Guid ProjectId, List<Widget> Widgets) : IRequest<bool>;

  public class Handler(IDatabaseService database) : IRequestHandler<Command, bool>
  {
    public Task<bool> Handle(Command request, CancellationToken token)
    {
      return database.UpdateWebProjectWidgetsAsync(request.ProjectId, request.Widgets, token);
    }
  }
}
