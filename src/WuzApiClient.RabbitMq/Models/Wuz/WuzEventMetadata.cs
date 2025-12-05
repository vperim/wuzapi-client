using System;
using System.Text.Json;
using WuzApiClient.RabbitMq.Models.Events;

namespace WuzApiClient.RabbitMq.Models.Wuz;

public static class WhatsAppEnvelopeSerializer
{
    /// <summary>
    /// Deserializes a WhatsApp event envelope from JSON data.
    /// ALL envelope types deserialize from root JSON - System.Text.Json automatically maps properties by name.
    /// </summary>
    public static TPayload? Deserialize<TPayload>(WuzEventEnvelopeMetadata envelopeMetadata)
        where TPayload : class, IWhatsAppEnvelope
    {
        // ALL envelopes deserialize from root JSON
        // System.Text.Json automatically maps:
        //   "type" → Type property
        //   "event" → Event property
        //   "base64" → Base64 property (if exists on envelope)
        return JsonSerializer.Deserialize<TPayload>(envelopeMetadata.JsonData);
    }
}

public sealed record WuzEventMetadata(WuzEventEnvelopeMetadata WuzEnvelope, WhatsAppEventMetadata WaEventMetadata)
{
    private static readonly JsonSerializerOptions Options = new() { PropertyNameCaseInsensitive = true };

    public static WuzEventMetadata Parse(ReadOnlyMemory<byte> body)
    {
        using var document = JsonDocument.Parse(body);
        var envelopeMetadata = document.Deserialize<WuzEventEnvelopeMetadata>(Options);
        if (envelopeMetadata == null)
            throw new WuzEventMetadataException("Envelope is null.");

        var waEventMetadata = WhatsAppEventMetadata.Parse(envelopeMetadata.JsonData);
        if (waEventMetadata == null)
            throw new WuzEventMetadataException("Payload is null.");

        return new WuzEventMetadata(envelopeMetadata, waEventMetadata);
    }

    public WuzEventEnvelope<TPayload> ToEnvelope<TPayload>()
        where TPayload : class, IWhatsAppEnvelope
    {
        var payload = WhatsAppEnvelopeSerializer.Deserialize<TPayload>(this.WuzEnvelope);
        if (payload is null)
        {
            throw new WuzEventMetadataException("Deserialized event returned null.");
        }

        return new WuzEventEnvelope<TPayload>
        {
            Metadata = this,
            Payload = payload,
            ReceivedAt = DateTimeOffset.UtcNow
        };
    }
}