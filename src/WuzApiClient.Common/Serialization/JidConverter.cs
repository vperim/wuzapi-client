using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using WuzApiClient.Common.Extensions;
using WuzApiClient.Common.Models;

namespace WuzApiClient.Common.Serialization;

/// <summary>
/// JSON converter for <see cref="Jid"/> type.
/// </summary>
public sealed class JidConverter : JsonConverter<Jid>
{
    /// <inheritdoc/>
    public override Jid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return default; // Return default(Jid) for null values

        var value = reader.GetString();

        if (value.IsNullOrEmpty())
            return default; // Treat empty strings as null for optional JID fields

        return new Jid(value);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, Jid value, JsonSerializerOptions options)
    {
        if (value.Value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value.Value);
    }

    /// <inheritdoc/>
    public override Jid ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (value.IsNullOrEmpty())
            throw new JsonException("Dictionary key cannot be null or empty.");

        return new Jid(value);
    }

    /// <inheritdoc/>
    public override void WriteAsPropertyName(Utf8JsonWriter writer, Jid value, JsonSerializerOptions options)
    {
        if (value.Value == null)
            throw new JsonException("Cannot serialize dictionary with null Jid key.");

        writer.WritePropertyName(value.Value);
    }
}
