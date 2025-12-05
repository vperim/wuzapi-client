using System;
using System.Text.Json;
using WuzApiClient.Extensions;

namespace WuzApiClient.RabbitMq.Models.Wuz;

public sealed record WhatsAppEventMetadata(string Event, string Type)
{
    public static WhatsAppEventMetadata Parse(string jsonData)
    {
        using var document = JsonDocument.Parse(jsonData);
        var root = document.RootElement;
        var eventValue = root.GetProperty("event").GetRawText(); // Serializes to JSON string
        var typeValue = root.GetProperty("type").GetString();
        if (typeValue.IsNullOrWhiteSpace())
            throw new WhatsAppEventMetadataException("Type property is null.");

        return new WhatsAppEventMetadata(eventValue, typeValue);
    }
}

public sealed class WhatsAppEventMetadataException : Exception
{
    public WhatsAppEventMetadataException(string message) : base(message)
    {
    }
}
