// Author: Alexander Dulemba
// Copyright 2023
// Derived from MongoDB docs: https://www.mongodb.com/docs/drivers/csharp/current/

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
    _logger.LogInformation("Getting web projects for profile {profileId}", profileId);

    return await _webProjects.AsQueryable()
      .Where(p => p.ProfileId == profileId)
      .ToListAsync(token);
  }

  public async Task<WebProject?> GetWebProjectAsync(Guid projectId, CancellationToken token)
  {
    _logger.LogInformation("Getting web project {projectId}", projectId);

    return await _webProjects.AsQueryable()
      .Where(p => p.Id == projectId)
      .FirstOrDefaultAsync(token);
  }

  public async Task CreateWebProjectAsync(WebProject project, CancellationToken token)
  {
    _logger.LogInformation("Creating new web project {projectId}", project.Id);

    await _webProjects.InsertOneAsync(project, null, token);
  }

  public async Task<bool> UpdateWebProjectAsync(WebProject project, CancellationToken token)
  {
    _logger.LogInformation("Updating web project {projectId}", project.Id);

    //var options = new ReplaceOptions() { IsUpsert = true };
    var result = await _webProjects.ReplaceOneAsync(
      p => p.Id == project.Id, 
      project, 
      cancellationToken: token);

    return result.IsAcknowledged;
  }

  public async Task<bool> UpdateWebProjectTitleAsync(Guid projectId, string newTitle, CancellationToken token)
  {
    _logger.LogInformation("Updating web project {projectId} title to {newTitle}", projectId, newTitle);

    var filter = Builders<WebProject>.Filter.Eq(wp => wp.Id, projectId);
    var updateTitle = Builders<WebProject>.Update.Set(wp => wp.Name, newTitle);
    var updateTime = Builders<WebProject>.Update.Set(wp => wp.LastModified, DateTime.Now);

    var titleUpdateResult = await _webProjects.UpdateOneAsync(filter, updateTitle, cancellationToken: token);
    var timeUpdateResult = await _webProjects.UpdateOneAsync(filter, updateTime, cancellationToken: token);

    return titleUpdateResult.IsAcknowledged && timeUpdateResult.IsAcknowledged;
  }

  public async Task<bool> UpdateWebProjectWidgetsAsync(Guid projectId, List<Widget> widgets, CancellationToken token)
  {
    _logger.LogInformation("Updating widgets for project {projectId}", projectId);

    var filter = Builders<WebProject>.Filter.Eq(wp => wp.Id, projectId);
    var updateWigdets = Builders<WebProject>.Update.Set(wp => wp.Widgets, widgets);
    var updateTime = Builders<WebProject>.Update.Set(wp => wp.LastModified, DateTime.Now);

    var widgetsUpdateResult = await _webProjects.UpdateOneAsync(filter, updateWigdets, cancellationToken: token);
    var timeUpdateResult = await _webProjects.UpdateOneAsync(filter, updateTime, cancellationToken: token);

    return widgetsUpdateResult.IsAcknowledged && timeUpdateResult.IsAcknowledged;
  }

  public async Task<bool> DeleteWebProjectAsync(Guid projectId, CancellationToken token)
  {
    _logger.LogInformation("Deleting web project {projectId}", projectId);

    var result = await _webProjects.DeleteOneAsync(p => p.Id == projectId, token);
    return result.IsAcknowledged;
  }
}
