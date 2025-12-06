using System;
using System.Text.Json;
using WuzApiClient.Common.Enums;
using WuzApiClient.Common.Extensions;

namespace WuzApiClient.RabbitMq.Models.Wuz;

public sealed record WhatsAppEventMetadata(string Event, WhatsAppEventType Type)
{
    public static WhatsAppEventMetadata Parse(JsonElement root)
    {
        var eventValue = root.GetProperty("event").GetRawText(); // Serializes to JSON string
        var typeValue = root.GetProperty("type").GetString();
        if (typeValue.IsNullOrWhiteSpace())
            throw new WhatsAppEventMetadataException("Type property is null.");

        if (!Enum.TryParse<WhatsAppEventType>(typeValue, out var eventType))
            eventType = WhatsAppEventType.Unknown;

        return new WhatsAppEventMetadata(eventValue, eventType);
    }
}

public sealed class WhatsAppEventMetadataException : Exception
{
    public WhatsAppEventMetadataException(string message) : base(message)
    {
    }
}
