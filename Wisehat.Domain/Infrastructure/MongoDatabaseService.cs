using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Wisehat.Domain.Entities;

namespace Wisehat.Domain.Infrastructure;

public class MongoDatabaseService : IDatabaseService
{
  public const string Name = "MongoConnection";
  public const string Database = "MongoDatabase";
  public const string Collection = "MongoCollection";

  private readonly IMongoClient _mongoClient;
  private readonly IMongoCollection<WebProject> _webProjects;
  private readonly ILogger<MongoDatabaseService> _logger;

  public MongoDatabaseService(IMongoClient mongoClient, string database, string collection, ILogger<MongoDatabaseService> logger)
  {
    _mongoClient = mongoClient;
    _webProjects = _mongoClient.GetDatabase(database).GetCollection<WebProject>(collection);
    _logger = logger;
  }

  public async Task<IEnumerable<WebProject>> GetWebProjectsByProfileIdAsync(string profileId, CancellationToken token)
  {
    return await _webProjects.AsQueryable()
      .Where(p => p.ProfileId == profileId)
      .ToListAsync(token);
  }

  public async Task<WebProject?> GetWebProjectAsync(Guid projectId, CancellationToken token)
  {
    return await _webProjects.AsQueryable()
      .Where(p => p.Id == projectId)
      .FirstOrDefaultAsync(token);
  }

  public async Task CreateWebProjectAsync(WebProject project, CancellationToken token)
  {
    await _webProjects.InsertOneAsync(project, null, token);
  }

  public async Task<bool> UpdateWebProjectAsync(WebProject webProject, CancellationToken token)
  {
    //var options = new ReplaceOptions() { IsUpsert = true };
    var result = await _webProjects.ReplaceOneAsync(
      p => p.Id == webProject.Id, 
      webProject, 
      cancellationToken: token);

    return result.IsAcknowledged;
  }

  public async Task<bool> DeleteWebProjectAsync(Guid projectId, CancellationToken token)
  {
    var result = await _webProjects.DeleteOneAsync(p => p.Id == projectId, token);
    return result.IsAcknowledged;
  }
}
