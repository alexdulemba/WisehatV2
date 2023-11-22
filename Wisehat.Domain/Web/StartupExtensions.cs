using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Wisehat.Domain.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using FluentValidation;
using Wisehat.Domain.Entities;
using Wisehat.Domain.Validators;

namespace Wisehat.Domain.Web;

public static class StartupExtensions
{
  public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddSingleton<IMongoClient>(_ =>
    {
      var connectionString = configuration.GetConnectionString(MongoDatabaseService.Name)
        ?? throw new NullReferenceException($"{MongoDatabaseService.Name} connection string not found.");

      return new MongoClient(MongoClientSettings.FromConnectionString(connectionString));
    });

    services.AddScoped<IDatabaseService>(serviceProvider =>
    {
      var database = configuration.GetConnectionString(MongoDatabaseService.Database)
        ?? throw new NullReferenceException();

      var collection = configuration.GetConnectionString(MongoDatabaseService.Collection)
        ?? throw new NullReferenceException();

      var client = serviceProvider.GetRequiredService<IMongoClient>();
      var logger = serviceProvider.GetRequiredService<ILogger<MongoDatabaseService>>();

      return new MongoDatabaseService(client, database, collection, logger);
    });

    services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining<IDatabaseService>()); 

    return services;
  }

  public static IServiceCollection AddDomainValidators(this IServiceCollection services)
  {
    services.AddValidatorsFromAssemblyContaining<WebProjectValidator>();
    
    return services;
  }
}
