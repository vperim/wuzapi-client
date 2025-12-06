using System.Text.Json.Serialization;
using WuzApiClient.Common.Models;
using WuzApiClient.Common.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when device pairing succeeds.
/// Corresponds to whatsmeow events.PairSuccess.
/// </summary>
public sealed record PairSuccessEventEnvelope : WhatsAppEventEnvelope<PairSuccessEventData>
{
    /// <summary>
    /// Gets the pair success event data.
    /// </summary>
    [JsonPropertyName("event")]
    public override required PairSuccessEventData Event { get; init; }
}

/// <summary>
/// Pair success event data from whatsmeow events.PairSuccess.
/// Contains information about the successfully paired device.
/// </summary>
public sealed record PairSuccessEventData
{
    /// <summary>
    /// Gets the JID (Jabber ID) of the paired device.
    /// </summary>
    [JsonPropertyName("ID")]
    [JsonConverter(typeof(JidConverter))]
    public Jid Jid { get; init; }

    /// <summary>
    /// Gets the business account ID (LID).
    /// </summary>
    [JsonPropertyName("LID")]
    public string? Lid { get; init; }

    /// <summary>
    /// Gets the business name.
    /// </summary>
    [JsonPropertyName("BusinessName")]
    public string? BusinessName { get; init; }

    /// <summary>
    /// Gets the platform identifier.
    /// </summary>
    [JsonPropertyName("Platform")]
    public string? Platform { get; init; }
}
