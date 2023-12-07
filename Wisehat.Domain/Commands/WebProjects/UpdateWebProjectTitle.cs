// Author: Alexander Dulemba
// Copyright 2023

using MediatR;
using Wisehat.Domain.Infrastructure;

namespace Wisehat.Domain.Commands.WebProjects;

public static class UpdateWebProjectTitle
{
  public record Command(Guid ProjectId, string NewTitle) : IRequest<bool>;

  public class Handler(IDatabaseService database) : IRequestHandler<Command, bool>
  {
    public Task<bool> Handle(Command request, CancellationToken token)
    {
      return database.UpdateWebProjectTitleAsync(request.ProjectId, request.NewTitle, token);
    }
  }
}
