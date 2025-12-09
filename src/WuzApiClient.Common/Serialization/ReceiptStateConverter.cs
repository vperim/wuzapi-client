using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using WuzApiClient.Common.Enums;

namespace WuzApiClient.Common.Serialization;

/// <summary>
/// JSON converter for ReceiptState enum that serializes as strings.
/// Maps wuzapi state values to strongly-typed enum values.
/// Returns Unknown for unrecognized values.
/// </summary>
public sealed class ReceiptStateConverter : JsonConverter<ReceiptState>
{
    /// <summary>
    /// Reads the JSON string and converts it to ReceiptState enum.
    /// Returns Unknown if the value is not recognized.
    /// </summary>
    public override ReceiptState Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return ReceiptState.Unknown;
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string token for ReceiptState, got {reader.TokenType}");
        }

        var value = reader.GetString();

        return value switch
        {
            "Read" => ReceiptState.Read,
            "ReadSelf" => ReceiptState.ReadSelf,
            "Delivered" => ReceiptState.Delivered,
            _ => ReceiptState.Unknown
        };
    }

    /// <summary>
    /// Writes the ReceiptState enum as a JSON string.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, ReceiptState value, JsonSerializerOptions options)
    {
        var stringValue = value switch
        {
            ReceiptState.Read => "Read",
            ReceiptState.ReadSelf => "ReadSelf",
            ReceiptState.Delivered => "Delivered",
            _ => ""
        };

        writer.WriteStringValue(stringValue);
    }
}
