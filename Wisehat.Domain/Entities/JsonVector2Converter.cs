// Author: Alexander Dulemba
// Copyright 2023

using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wisehat.Domain.Entities;

public class JsonVector2Converter : JsonConverter<Vector2>
{
  public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    if (reader.TokenType != JsonTokenType.StartObject)
      throw new JsonException();

    var vectorProperties = new Dictionary<string, float>();
    while (reader.Read())
    {
      if (reader.TokenType == JsonTokenType.EndObject)
        return BuildVector2(vectorProperties);

      if (reader.TokenType != JsonTokenType.PropertyName)
        throw new JsonException();

      var propertyName = reader.GetString()?.ToLower() ?? throw new JsonException();
      reader.Read();
      var value = reader.GetSingle();
      vectorProperties.Add(propertyName, value);
    }

    throw new JsonException();
  }

  private static Vector2 BuildVector2(Dictionary<string, float> properties)
  {
    if (properties.TryGetValue("x", out var x) && properties.TryGetValue("y", out var y))
    {
      return new Vector2(x, y);
    }

    return new Vector2(0, 0);
  }

  public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
  {
    writer.WriteStartObject();

    writer.WritePropertyName("X");
    writer.WriteNumberValue(value.X);

    writer.WritePropertyName("Y");
    writer.WriteNumberValue(value.Y);

    writer.WriteEndObject();
  }
}
