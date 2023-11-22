using MongoDB.Bson.Serialization.Attributes;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Wisehat.Domain.Entities
{
  [JsonConverter(typeof(JsonWidgetConverter))]
  [BsonDiscriminator(Required = true)]
  [BsonKnownTypes(typeof(FillBox), typeof(ImageBox), typeof(TextBox), typeof(VideoBox))]
  public abstract class Widget
  {
    public Guid Id { get; set; }

    public string? Description { get; set; }

    public abstract WidgetType Type { get; }

    [JsonConverter(typeof(JsonVector2Converter))]
    public Vector2 Position { get; set; }

    [JsonConverter(typeof(JsonVector2Converter))]
    public Vector2 Size { get; set; }

    public string? Content { get; set; }

    public string BackgroundColor { get; set; } = "#252525";

    public string BorderColor { get; set; } = "#252525";
  }
}
