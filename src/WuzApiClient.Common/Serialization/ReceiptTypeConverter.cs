using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using WuzApiClient.Common.Enums;

namespace WuzApiClient.Common.Serialization;

/// <summary>
/// JSON converter for ReceiptType enum that serializes as strings.
/// Maps whatsmeow string values to strongly-typed enum values.
/// Returns Unknown for unrecognized values.
/// </summary>
public sealed class ReceiptTypeConverter : JsonConverter<ReceiptType>
{
    /// <summary>
    /// Reads the JSON string and converts it to ReceiptType enum.
    /// Returns Unknown if the value is not recognized.
    /// </summary>
    public override ReceiptType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return ReceiptType.Unknown;
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string token for ReceiptType, got {reader.TokenType}");
        }

        var value = reader.GetString();

        return value switch
        {
            "" => ReceiptType.Delivered,
            "sender" => ReceiptType.Sender,
            "retry" => ReceiptType.Retry,
            "read" => ReceiptType.Read,
            "read-self" => ReceiptType.ReadSelf,
            "played" => ReceiptType.Played,
            "played-self" => ReceiptType.PlayedSelf,
            "server-error" => ReceiptType.ServerError,
            "inactive" => ReceiptType.Inactive,
            "peer_msg" => ReceiptType.PeerMsg,
            "hist_sync" => ReceiptType.HistorySync,
            _ => ReceiptType.Unknown
        };
    }

    /// <summary>
    /// Writes the ReceiptType enum as a JSON string.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, ReceiptType value, JsonSerializerOptions options)
    {
        var stringValue = value switch
        {
            ReceiptType.Delivered => "",
            ReceiptType.Sender => "sender",
            ReceiptType.Retry => "retry",
            ReceiptType.Read => "read",
            ReceiptType.ReadSelf => "read-self",
            ReceiptType.Played => "played",
            ReceiptType.PlayedSelf => "played-self",
            ReceiptType.ServerError => "server-error",
            ReceiptType.Inactive => "inactive",
            ReceiptType.PeerMsg => "peer_msg",
            ReceiptType.HistorySync => "hist_sync",
            _ => ""
        };

        writer.WriteStringValue(stringValue);
    }
}
