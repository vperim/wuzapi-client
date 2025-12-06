using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using WuzApiClient.Common.Models;

namespace WuzApiClient.Common.Serialization;

/// <summary>
/// JSON converter for Jid[] arrays.
/// Implements fail-fast error handling for invalid JIDs.
/// </summary>
public sealed class JidArrayConverter : JsonConverter<Jid[]?>
{
    public override Jid[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException("Expected array start token for Jid[].");

        var list = new List<Jid>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                return list.ToArray();

            // Handle null elements in array
            if (reader.TokenType == JsonTokenType.Null)
            {
                list.Add(default); // Add default(Jid) for null array elements
                continue;
            }

            var value = reader.GetString();

            // CRITICAL: Fail-fast instead of silently skipping (Architect Issue #2)
            if (string.IsNullOrEmpty(value))
                throw new JsonException("Array contains null or empty JID value.");

            if (!Jid.TryParse(value, out var jid))
                throw new JsonException($"Invalid JID format in array: {value}");

            list.Add(jid);
        }

        throw new JsonException("Unexpected end of JSON while reading Jid[].");
    }

    public override void Write(Utf8JsonWriter writer, Jid[]? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartArray();

        foreach (var jid in value)
        {
            if (jid.Value == null)
                writer.WriteNullValue();
            else
                writer.WriteStringValue(jid.Value);
        }

        writer.WriteEndArray();
    }
}
