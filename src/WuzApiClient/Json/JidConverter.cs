using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using WuzApiClient.Models.Common;

namespace WuzApiClient.Json;

/// <summary>
/// JSON converter for <see cref="Jid"/> type.
/// </summary>
public sealed class JidConverter : JsonConverter<Jid>
{
    /// <inheritdoc/>
    public override Jid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (string.IsNullOrEmpty(value))
            throw new JsonException("JID cannot be null or empty.");

        return new Jid(value);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, Jid value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
