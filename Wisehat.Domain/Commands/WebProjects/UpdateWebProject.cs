// Author: Alexander Dulemba
// Copyright 2023

using FluentValidation;
using MediatR;
using Wisehat.Domain.Entities;
using Wisehat.Domain.Infrastructure;

namespace Wisehat.Domain.Commands.WebProjects;

public static class UpdateWebProject
{
  public record Command(WebProject Project) : IRequest<bool>;

  public class Handler(IDatabaseService database, IValidator<WebProject> validator)
    : IRequestHandler<Command, bool>
  {
    public Task<bool> Handle(Command command, CancellationToken token) 
    {
      validator.ValidateAndThrow(command.Project);
      command.Project.LastModified = DateTime.Now;

      return database.UpdateWebProjectAsync(command.Project, token);
    }
  }
}
