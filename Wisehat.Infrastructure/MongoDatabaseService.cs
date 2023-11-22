using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Wisehat.Domain;
using Wisehat.Domain.Entities;

namespace Wisehat.Infrastructure
{
  public class MongoDatabaseService : IDatabaseService
  {
    private readonly IMongoClient _mongoClient;
    private readonly IMongoCollection<WebProject> _webProjects;
    private readonly ILogger<MongoDatabaseService> _logger;

    public MongoDatabaseService(IMongoClient mongoClient, string database, string collection, ILogger<MongoDatabaseService> logger)
    {
      _mongoClient = mongoClient;
      _webProjects = _mongoClient.GetDatabase(database).GetCollection<WebProject>(collection);
      _logger = logger;
    }

    public async Task<List<WebProject>> GetWebProjectsByProfileIdAsync(string profileId)
    {
      return await _webProjects.AsQueryable()
        .Where(p => p.ProfileId == profileId)
        .ToListAsync();
    }

    public async Task<WebProject?> GetWebProjectAsync(Guid projectId)
    {
      return await _webProjects.AsQueryable()
        .Where(p => p.Id == projectId)
        .FirstOrDefaultAsync();
    }

    public async Task<bool> CreateWebProjectAsync(WebProject project)
    {
      await _webProjects.InsertOneAsync(project);

      var result = GetWebProjectAsync(project.Id);
      return result is not null;
    }

    public async Task<bool> UpdateWebProjectAsync(WebProject webProject)
    {
      //var options = new ReplaceOptions() { IsUpsert = true };
      var result = await _webProjects.ReplaceOneAsync(p => p.Id == webProject.Id, webProject);
      return result.IsAcknowledged;
    }

    public async Task<bool> DeleteWebProjectAsync(Guid projectId)
    {
      var result = await _webProjects.DeleteOneAsync(p => p.Id == projectId);
      return result.IsAcknowledged;
    }
  }
}
