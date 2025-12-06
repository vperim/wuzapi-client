using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using WuzApiClient.Common.Enums;
using WuzApiClient.Models.Common;

namespace WuzApiClient.Json;

/// <summary>
/// Converts WhatsAppEventType[] to/from JSON string array using enum member names.
/// </summary>
public sealed class SubscribableEventArrayConverter : JsonConverter<WhatsAppEventType[]>
{
    public override WhatsAppEventType[] Read(
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

        var list = new List<WhatsAppEventType>();
        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            var value = reader.GetString();
            if (Enum.TryParse<WhatsAppEventType>(value, ignoreCase: false, out var evt))
            {
                list.Add(evt);
            }
        }

        return list.ToArray();
    }

    public override void Write(
        Utf8JsonWriter writer,
        WhatsAppEventType[] value,
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
