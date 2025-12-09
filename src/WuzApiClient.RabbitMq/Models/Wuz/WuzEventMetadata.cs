using System;
using System.Text.Json;

namespace WuzApiClient.RabbitMq.Models.Wuz;

/// <summary>
/// Combined metadata from a wuzapi RabbitMQ message.
/// Wuzapi sends a flattened JSON structure where transport metadata (userID, instanceName)
/// and WhatsApp event data (type, event, etc.) are all at the root level.
/// </summary>
/// <param name="WuzEnvelope">Transport metadata (userID, instanceName).</param>
/// <param name="WaEventMetadata">WhatsApp event metadata (type, event).</param>
/// <param name="RawJson">The original JSON string for payload deserialization.</param>
public sealed record WuzEventMetadata(
    WuzEventEnvelopeMetadata WuzEnvelope,
    WhatsAppEventMetadata WaEventMetadata,
    string RawJson)
{
    private static readonly JsonSerializerOptions Options = new() { PropertyNameCaseInsensitive = true };

    /// <summary>
    /// Parses the raw RabbitMQ message body into metadata.
    /// Expects flattened JSON with userID, instanceName, type, and event at root level.
    /// </summary>
    /// <param name="body">The raw message bytes.</param>
    /// <returns>Parsed metadata.</returns>
    /// <exception cref="WuzEventMetadataException">Thrown when required fields are missing.</exception>
    public static WuzEventMetadata Parse(ReadOnlyMemory<byte> body)
    {
        using var document = JsonDocument.Parse(body);
        var root = document.RootElement;

        var envelopeMetadata = document.Deserialize<WuzEventEnvelopeMetadata>(Options);
        if (envelopeMetadata == null) throw new WuzEventMetadataException("WuzEventEnvelopeMetadata is null.");

        // Extract WhatsApp event metadata from root level
        var waEventMetadata = WhatsAppEventMetadata.Parse(root);

        return new WuzEventMetadata(envelopeMetadata, waEventMetadata, root.GetRawText());
    }

    /// <summary>
    /// Creates a typed event envelope from this metadata.
    /// </summary>
    /// <typeparam name="TPayload">The payload type implementing IWhatsAppEventEnvelope.</typeparam>
    /// <returns>A typed event envelope.</returns>
    /// <exception cref="WuzEventMetadataException">Thrown when deserialization fails.</exception>
    public WuzEventEnvelope<TPayload> ToEnvelope<TPayload>()
        where TPayload : class, IWhatsAppEventEnvelope
    {
        // Deserialize the payload directly from the flattened JSON
        var payload = JsonSerializer.Deserialize<TPayload>(this.RawJson, Options);
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
