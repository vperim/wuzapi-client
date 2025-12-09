using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using WuzApiClient.Common.Enums;

namespace WuzApiClient.Common.Serialization;

/// <summary>
/// JSON converter for ConnectFailureReason enum that serializes as integers.
/// Returns Unknown for unrecognized values.
/// </summary>
public sealed class ConnectFailureReasonConverter : JsonConverter<ConnectFailureReason>
{
    /// <summary>
    /// Reads the JSON integer and converts it to ConnectFailureReason enum.
    /// Returns Unknown if the value is not a defined enum member.
    /// </summary>
    public override ConnectFailureReason Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.Number)
        {
            throw new JsonException($"Expected number token for ConnectFailureReason, got {reader.TokenType}");
        }

        var value = reader.GetInt32();

        if (Enum.IsDefined(typeof(ConnectFailureReason), value))
            return (ConnectFailureReason)value;

        return ConnectFailureReason.Unknown;
    }

    /// <summary>
    /// Writes the ConnectFailureReason enum as a JSON integer.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, ConnectFailureReason value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue((int)value);
    }
}
