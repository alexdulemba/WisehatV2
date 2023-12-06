using MediatR;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using Wisehat.Domain.Commands.WebProjects;
using Wisehat.Domain.Entities;
using Wisehat.Web.Services;

namespace Wisehat.Web.Hubs;

public class WebProjectHub : Hub<IWebProjectCommands>
{
  private readonly EditorWidgetCacheService _widgetBucketService;
  private readonly ISender _sender;
  private readonly ILogger<WebProjectHub> _logger;

  public WebProjectHub(EditorWidgetCacheService widgetBucketService, ISender sender, ILogger<WebProjectHub> logger)
  {
    _widgetBucketService = widgetBucketService;
    _sender = sender;
    _logger = logger;
  }

  public async Task JoinWebProjectGroup(Guid projectId)
  {
    await Groups.AddToGroupAsync(Context.ConnectionId, projectId.ToString());
    _logger.LogInformation("New connection {connectionId} added to group {groupId}", Context.ConnectionId, projectId);
  }

  public async Task LeaveWebProjectGroup(Guid projectId)
  {
    await Groups.RemoveFromGroupAsync(Context.ConnectionId, projectId.ToString());
    _logger.LogInformation("Removed connection from {groupId}", projectId);
  }

  public async Task UpdateWebProjectTitle(Guid projectId, string newProjectTitle)
  {
    _logger.LogInformation("Project {projectId} title updated to {newProjectTitle}, sending new info to other open editors...", projectId, newProjectTitle);
    await Clients.OthersInGroup(projectId.ToString())
      .IncomingNewWebProjectTitle(projectId, newProjectTitle);
  }

  public void AddWidgetToWebProject(Guid projectId, Widget widget)
  {
    _logger.LogInformation("Received widget {widgetId} for project {projectId}", widget.Id, projectId);
    _widgetBucketService.AddWidget(projectId, widget);
  }

  public void UpdateWidgetPosition(Guid widgetId, float positionX, float positionY)
  {
    _logger.LogInformation("Updating stored widget position");
    _widgetBucketService.UpdateWidgetPosition(widgetId, positionX, positionY);
  }

  public void UpdateWidgetSize(Guid widgetId, float newWidth, float newHeight)
  {
    _logger.LogInformation("Updating stored widget size");
    _widgetBucketService.UpdateWidgetSize(widgetId, newWidth, newHeight);
  }

  public void UpdateWidgetFillColor(Guid widgetId, string hexColor)
  {
    _logger.LogInformation("Updating stored widget fill color");
    _widgetBucketService.UpdateWidgetFillColor(widgetId, hexColor);
  }

  public void RemoveWidgetFromWebProject(Guid projectId, Guid widgetId)
  {
    _logger.LogInformation("Removing widget {widgetId} from project {projectId}", widgetId, projectId);
    _widgetBucketService.RemoveWidget(projectId, widgetId);
  }

  public void RemoveAllWidgetsFromWebProject(Guid projectId)
  {
    _logger.LogInformation("Removing all widgets from project {projectId}", projectId);
    _widgetBucketService.RemoveAllWidgetsForProject(projectId);
  }

  public async Task SaveWebProjectAsync(Guid projectId)
  {
    var widgets = _widgetBucketService.GetWidgetsByProject(projectId);
    var command = new UpdateWebProjectWidgets.Command(projectId, widgets);
    var success = await _sender.Send(command);
    _logger.LogInformation("Widgets data {result}", success ? "saved successfully" : "failed to save");
  }
}

public interface IWebProjectCommands
{
  Task IncomingNewWebProjectTitle(Guid id, string message);
}
