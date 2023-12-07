// Author: Alexander Dulemba
// Copyright 2023

using FluentValidation;
using MediatR;
using Wisehat.Domain.Entities;
using Wisehat.Domain.Infrastructure;

namespace Wisehat.Domain.Commands.WebProjects;

public static class CreateWebProject  
{
  public record Command(string Name, string UserId) : IRequest<WebProject>;

  public class Handler(IDatabaseService database, IValidator<WebProject> validator) 
    : IRequestHandler<Command, WebProject>
  {
    public async Task<WebProject> Handle(Command request, CancellationToken token)
    {
      var newProject = new WebProject()
      {
        Id = Guid.NewGuid(),
        ProfileId = request.UserId,
        Name = request.Name,
        CreatedOn = DateTime.Now,
        LastModified = DateTime.Now,
      };

      validator.ValidateAndThrow(newProject);

      await database.CreateWebProjectAsync(newProject, token);

      return newProject;
    }
  }
}

