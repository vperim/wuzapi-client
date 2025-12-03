using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using WuzApiClient.Models.Common;

namespace WuzApiClient.Json;

/// <summary>
/// Converts SubscribableEvent[] to/from JSON string array using enum member names.
/// </summary>
public sealed class SubscribableEventArrayConverter : JsonConverter<SubscribableEvent[]>
{
    public override SubscribableEvent[] Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return [];
        }

        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException("Expected array");
        }

        var list = new List<SubscribableEvent>();
        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            var value = reader.GetString();
            if (Enum.TryParse<SubscribableEvent>(value, ignoreCase: false, out var evt))
            {
                list.Add(evt);
            }
        }

        return list.ToArray();
    }

    public override void Write(
        Utf8JsonWriter writer,
        SubscribableEvent[] value,
        JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var evt in value)
        {
            writer.WriteStringValue(evt.ToString());
        }
        writer.WriteEndArray();
    }
}
