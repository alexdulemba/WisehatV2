// Author: Alexander Dulemba
// Copyright 2023

using System.Numerics;
using System.Text.Json.Serialization;

namespace Wisehat.Domain.Entities;

[JsonConverter(typeof(JsonWidgetConverter))]
public class Widget
{
  public Guid Id { get; set; }

  public string? Description { get; set; }

  public WidgetType Type { get; set; }

  [JsonConverter(typeof(JsonVector2Converter))]
  public Vector2 GrabPosition { get; set; }

  [JsonConverter(typeof(JsonVector2Converter))]
  public Vector2 Size { get; set; }

  public string? Content { get; set; }

  public string BackgroundColor { get; set; } = "#252525";

  public string Border { get; set; } = "1px solid #252525";

  [JsonConverter(typeof(JsonVector2Converter))]
  public Vector2 Position { get; set; }
}
