﻿// Author: Alexander Dulemba
// Copyright 2023

using System.Numerics;
using Wisehat.Domain.Entities;

namespace Wisehat.Web.Services;

public class EditorWidgetCacheService
{
  private readonly List<(Guid ProjectId, Widget Widget)> _projectWidgets = [];
  private readonly ILogger<EditorWidgetCacheService> _logger;

  public EditorWidgetCacheService(ILogger<EditorWidgetCacheService> logger)
  {
    _logger = logger;
  }

  public List<Widget> GetWidgetsByProject(Guid projectId) 
  { 
    return _projectWidgets.Where(x => x.ProjectId == projectId)
      .Select(x => x.Widget)
      .ToList();
  }

  public void AddWidget(Guid projectId, Widget widget)
  {
    _projectWidgets.Add((projectId, widget));
    _logger.LogInformation("Stored new widget");
  }

  public void AddWidgets(Guid projectId, IEnumerable<Widget> widgets) 
  { 
    foreach (var widget in widgets) 
    { 
      _projectWidgets.Add((projectId, widget));
    }
    _logger.LogInformation("Stored new widgets");
  }

  public void UpdateWidgetPosition(Guid widgetId, float positionX, float positionY)
  {
    var widget = _projectWidgets.FirstOrDefault(x => x.Widget.Id == widgetId).Widget;
    widget.Position = new Vector2(positionX, positionY);
    _logger.LogInformation("Updated widget {widgetId} position", widgetId);
  }

  public void UpdateWidgetSize(Guid widgetId, float newWidth, float newHeight)
  {
    var widget = _projectWidgets.FirstOrDefault(x => x.Widget.Id == widgetId).Widget;
    if (widget != null) // ResizeObserver gets called when widget is deleted from canvas, thus calling this method when widget has been deleted already
    {
      widget.Size = new Vector2(newWidth, newHeight);
      _logger.LogInformation("Updated widget {widgetId} size", widgetId);
    }
  }

  public void UpdateWidgetFillColor(Guid widgetId, string hexColor)
  {
    var widget = _projectWidgets.FirstOrDefault(x => x.Widget.Id == widgetId).Widget;
    widget.BackgroundColor = hexColor;
    _logger.LogInformation("Updated widget {widgetId} fill color", widgetId);
  }

  public void UpdateWidgetBorder(Guid widgetId, string borderValue)
  {
    var widget = _projectWidgets.FirstOrDefault(x => x.Widget.Id == widgetId).Widget;
    widget.Border = borderValue;
    _logger.LogInformation("Updated widget {widgetId} border", widgetId);
  }

  public void UpdateWidgetContent(Guid widgetId, string content)
  {
    var widget = _projectWidgets.FirstOrDefault(x => x.Widget.Id == widgetId).Widget;
    widget.Content = content;
    _logger.LogInformation("Updated widget {widgetId} content", widgetId);
  }

  public void RemoveWidget(Guid projectId, Guid widgetId) 
  { 
    var widget = _projectWidgets.FirstOrDefault(x => x.Widget.Id == widgetId);
    _projectWidgets.Remove((projectId, widget.Widget));
    _logger.LogInformation("Removed widget {widgetId} from project {projectId}", widgetId, projectId);
  }

  public void RemoveAllWidgetsForProject(Guid projectId) 
  {
    _projectWidgets.RemoveAll(x => x.ProjectId == projectId);
    _logger.LogInformation("Cleared cached widgets for project {projectId}", projectId);
  }
}
