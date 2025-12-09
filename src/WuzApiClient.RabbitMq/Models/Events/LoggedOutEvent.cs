using System.Text.Json.Serialization;
using WuzApiClient.Common.Enums;
using WuzApiClient.Common.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Data for the event when the client is logged out from WhatsApp.
/// Corresponds to whatsmeow events.LoggedOut.
/// </summary>
public sealed record LoggedOutEventData
{
    /// <summary>
    /// Gets the reason for logout.
    /// </summary>
    [JsonPropertyName("reason")]
    [JsonConverter(typeof(ConnectFailureReasonConverter))]
    public ConnectFailureReason Reason { get; init; }
}

/// <summary>
/// Envelope for the event when the client is logged out from WhatsApp.
/// Corresponds to whatsmeow events.LoggedOut.
/// </summary>
public sealed record LoggedOutEventEnvelope : WhatsAppEventEnvelope<LoggedOutEventData>
{
    [JsonPropertyName("event")]
    public override required LoggedOutEventData Event { get; init; }
}
