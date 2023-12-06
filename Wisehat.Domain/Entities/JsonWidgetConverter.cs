using System.Numerics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wisehat.Domain.Entities;

public class JsonWidgetConverter : JsonConverter<Widget>
{
  private static readonly PropertyInfo[] _widgetProperties = typeof(Widget).GetProperties();

  public override Widget? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    if (reader.TokenType != JsonTokenType.StartObject)
      throw new JsonException();

    var objectProperties = new Dictionary<string, object>();
    while (reader.Read())
    {
      if (reader.TokenType == JsonTokenType.EndObject)
        return BuildWidget(objectProperties);

      if (reader.TokenType != JsonTokenType.PropertyName)
        throw new JsonException();

      var propertyName = reader.GetString()!.ToLower();
      reader.Read();

      var matchingWidgetProp = _widgetProperties.Where(p => p.Name.ToLower() == propertyName)
                                                .FirstOrDefault();

      if (matchingWidgetProp?.PropertyType == typeof(Vector2))
      {
        var vectorSerializer = new JsonVector2Converter();
        var vector = vectorSerializer.Read(ref reader, null!, null!);
        objectProperties.Add(propertyName, vector);
      }
      else
      {
        objectProperties.Add(propertyName, reader.GetString() ?? string.Empty);
      }
    }

    throw new JsonException();
  }

  private static Widget BuildWidget(Dictionary<string, object> properties)
  {
    if (properties.Count != _widgetProperties.Length)
      throw new JsonException();

    return new Widget()
    {
      Id = Guid.Parse(properties["id"].ToString()!),
      Description = properties["description"].ToString(),
      Type = Enum.Parse<WidgetType>(properties["type"].ToString()!, true),
      GrabPosition = properties["grabposition"].ToVector2(),
      Size = properties["size"].ToVector2(),
      Content = properties["content"].ToString(),
      BackgroundColor = properties["backgroundcolor"].ToString()!,
      Border = properties["bordercolor"].ToString()!,
      Position = properties["position"].ToVector2(),
    };
  }

  public override void Write(Utf8JsonWriter writer, Widget value, JsonSerializerOptions options)
  {
    writer.WriteStartObject();

    writer.WritePropertyName("_t");
    writer.WriteStringValue(value.GetType().Name);

    writer.WritePropertyName("_id");
    writer.WriteStringValue(value.Id);

    writer.WritePropertyName("Description");
    writer.WriteStringValue(value.Description);

    writer.WriteStartObject("Position");
    writer.WritePropertyName("X");
    writer.WriteNumberValue(value.GrabPosition.X);
    writer.WritePropertyName("Y");
    writer.WriteNumberValue(value.GrabPosition.Y);
    writer.WriteEndObject();

    writer.WriteStartObject("Size");
    writer.WritePropertyName("X");
    writer.WriteNumberValue(value.Size.X);
    writer.WritePropertyName("Y");
    writer.WriteNumberValue(value.Size.Y);
    writer.WriteEndObject();

    writer.WritePropertyName("Content");
    writer.WriteStringValue(value.Content);

    writer.WritePropertyName("BackgroundColor");
    writer.WriteStringValue(value.BackgroundColor);

    writer.WritePropertyName("BorderColor");
    writer.WriteStringValue(value.Border);

    writer.WriteStartObject("Location");
    writer.WritePropertyName("X");
    writer.WriteNumberValue(value.Size.X);
    writer.WritePropertyName("Y");
    writer.WriteNumberValue(value.Size.Y);
    writer.WriteEndObject();

    writer.WriteEndObject();
  }
}
