using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using WuzApiClient.Common.Models;

namespace WuzApiClient.Common.Serialization;

/// <summary>
/// JSON converter for Dictionary&lt;Jid, TValue&gt;.
/// Preserves null values to maintain standard dictionary behavior.
/// </summary>
/// <typeparam name="TValue">The value type of the dictionary.</typeparam>
public sealed class JidDictionaryConverter<TValue> : JsonConverter<Dictionary<Jid, TValue>?>
{
    public override Dictionary<Jid, TValue>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected object start token for Dictionary<Jid, TValue>.");

        var dictionary = new Dictionary<Jid, TValue>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                return dictionary;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected property name token.");

            var key = reader.GetString();

            if (string.IsNullOrEmpty(key))
                throw new JsonException("Dictionary key cannot be null or empty.");

            if (!Jid.TryParse(key, out var jid))
                throw new JsonException($"Invalid JID format for dictionary key: {key}");

            reader.Read();

            // CRITICAL: Preserve null values (Architect Issue #4)
            var value = JsonSerializer.Deserialize<TValue>(ref reader, options);
            dictionary[jid] = value!; // Allow null values in dictionary
        }

        throw new JsonException("Unexpected end of JSON while reading Dictionary<Jid, TValue>.");
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<Jid, TValue>? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();

        foreach (var kvp in value)
        {
            if (kvp.Key.Value == null)
                throw new JsonException("Cannot serialize dictionary with null Jid key.");

            writer.WritePropertyName(kvp.Key.Value);
            JsonSerializer.Serialize(writer, kvp.Value, options);
        }

        writer.WriteEndObject();
    }
}
