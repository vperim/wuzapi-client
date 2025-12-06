using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using WuzApiClient.Common.Enums;

namespace WuzApiClient.Common.Serialization;

/// <summary>
/// JSON converter for WhatsAppEventType enum that serializes as strings.
/// Enum member names match the JSON string values exactly (1:1 mapping).
/// </summary>
public sealed class WhatsAppEventTypeConverter : JsonConverter<WhatsAppEventType>
{
    /// <summary>
    /// Reads the JSON string and converts it to WhatsAppEventType enum.
    /// Returns Unknown if the value is null, empty, or not recognized.
    /// </summary>
    public override WhatsAppEventType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string token for WhatsAppEventType, got {reader.TokenType}");
        }

        var typeString = reader.GetString();

        if (string.IsNullOrWhiteSpace(typeString))
            return WhatsAppEventType.Unknown;

        if (Enum.TryParse<WhatsAppEventType>(typeString, out var eventType))
            return eventType;

        return WhatsAppEventType.Unknown;
    }

    /// <summary>
    /// Writes the WhatsAppEventType enum as a JSON string.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, WhatsAppEventType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
