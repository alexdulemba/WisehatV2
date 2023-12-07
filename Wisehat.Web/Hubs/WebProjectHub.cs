// Author: Alexander Dulemba
// Copyright 2023

// Code designed based on SignalR tutorial from Microsoft: https://learn.microsoft.com/en-us/aspnet/core/blazor/tutorials/signalr-blazor?view=aspnetcore-8.0&tabs=visual-studio

using MediatR;
using Microsoft.AspNetCore.SignalR;
using Wisehat.Domain.Commands.WebProjects;
using Wisehat.Domain.Entities;
using Wisehat.Web.Services;

namespace Wisehat.Web.Hubs;

public class WebProjectHub : Hub<IWebProjectCommands>
{
  private readonly EditorWidgetCacheService _widgetCache;
  private readonly ISender _sender;
  private readonly ILogger<WebProjectHub> _logger;

  public WebProjectHub(EditorWidgetCacheService widgetCache, ISender sender, ILogger<WebProjectHub> logger)
  {
    _widgetCache = widgetCache;
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
    _widgetCache.AddWidget(projectId, widget);
  }

  public void UpdateWidgetPosition(Guid widgetId, float positionX, float positionY)
  {
    _logger.LogInformation("Updating stored widget position");
    _widgetCache.UpdateWidgetPosition(widgetId, positionX, positionY);
  }

  public void UpdateWidgetSize(Guid widgetId, float newWidth, float newHeight)
  {
    _logger.LogInformation("Updating stored widget size");
    _widgetCache.UpdateWidgetSize(widgetId, newWidth, newHeight);
  }

  public void UpdateWidgetFillColor(Guid widgetId, string hexColor)
  {
    _logger.LogInformation("Updating stored widget fill color");
    _widgetCache.UpdateWidgetFillColor(widgetId, hexColor);
  }

  public void UpdateWidgetBorder(Guid widgetId, string borderValue)
  {
    _logger.LogInformation("Updating stored widget border");
    _widgetCache.UpdateWidgetBorder(widgetId, borderValue);
  }

  public void UpdateWidgetContent(Guid widgetId, string content)
  {
    _logger.LogInformation("Updating stored widget content");
    _widgetCache.UpdateWidgetContent(widgetId, content);
  }

  public void RemoveWidgetFromWebProject(Guid projectId, Guid widgetId)
  {
    _logger.LogInformation("Removing widget {widgetId} from project {projectId}", widgetId, projectId);
    _widgetCache.RemoveWidget(projectId, widgetId);
  }

  public void RemoveAllWidgetsFromWebProject(Guid projectId)
  {
    _logger.LogInformation("Removing all widgets from project {projectId}", projectId);
    _widgetCache.RemoveAllWidgetsForProject(projectId);
  }

  public async Task SaveWebProjectAsync(Guid projectId)
  {
    var widgets = _widgetCache.GetWidgetsByProject(projectId);
    var command = new UpdateWebProjectWidgets.Command(projectId, widgets);
    var success = await _sender.Send(command);
    _logger.LogInformation("Widgets data {result}", success ? "saved successfully" : "failed to save");
  }
}

public interface IWebProjectCommands
{
  Task IncomingNewWebProjectTitle(Guid id, string message);
}
