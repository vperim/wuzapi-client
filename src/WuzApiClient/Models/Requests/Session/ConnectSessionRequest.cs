using System.Text.Json.Serialization;
using WuzApiClient.Json;
using WuzApiClient.Models.Common;

namespace WuzApiClient.Models.Requests.Session;

/// <summary>
/// Request to connect a WhatsApp session.
/// </summary>
public sealed class ConnectSessionRequest
{
    /// <summary>
    /// Default subscription: all events.
    /// </summary>
    private static readonly SubscribableEvent[] DefaultSubscription = [SubscribableEvent.All];

    /// <summary>
    /// Gets or sets the events to subscribe to.
    /// Default: [SubscribableEvent.All] (subscribes to all events).
    /// To opt-out of event subscriptions, set to an empty array: [].
    /// </summary>
    [JsonPropertyName("subscribe")]
    [JsonConverter(typeof(SubscribableEventArrayConverter))]
    public SubscribableEvent[] Subscribe { get; set; } = DefaultSubscription;

    /// <summary>
    /// Gets or sets a value indicating whether to connect immediately.
    /// </summary>
    [JsonPropertyName("immediate")]
    public bool Immediate { get; set; } = true;
}
