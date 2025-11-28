using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using WuzApiClient.RabbitMq.Models;
using WuzApiClient.RabbitMq.Models.Events;

namespace WuzApiClient.RabbitMq.Serialization;

/// <summary>
/// JSON converter for polymorphic WuzEvent deserialization.
/// </summary>
public sealed class WuzEventJsonConverter : JsonConverter<WuzEvent>
{
    /// <inheritdoc/>
    public override WuzEvent? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        // Read the "type" field to determine which concrete event type to deserialize
        if (!root.TryGetProperty("type", out var typeElement))
        {
            throw new JsonException("Event JSON is missing the 'type' field.");
        }

        var eventType = typeElement.GetString();
        if (string.IsNullOrWhiteSpace(eventType))
        {
            throw new JsonException("Event 'type' field is null or empty.");
        }

        // Extract the "event" field which contains the actual event data
        if (!root.TryGetProperty("event", out var eventElement))
        {
            throw new JsonException("Event JSON is missing the 'event' field.");
        }

        // Get userID and instanceName from root
        var userId = root.TryGetProperty("userID", out var userIdElement)
            ? userIdElement.GetString() ?? string.Empty
            : string.Empty;
        var instanceName = root.TryGetProperty("instanceName", out var instanceElement)
            ? instanceElement.GetString() ?? string.Empty
            : string.Empty;

        // Map event type string to concrete event class
        var targetType = eventType switch
        {
            _ when string.Equals(eventType, EventTypes.Message, StringComparison.OrdinalIgnoreCase)
                => typeof(MessageEvent),
            _ when string.Equals(eventType, EventTypes.UndecryptableMessage, StringComparison.OrdinalIgnoreCase)
                => typeof(UndecryptableMessageEvent),
            _ when string.Equals(eventType, EventTypes.Receipt, StringComparison.OrdinalIgnoreCase)
                => typeof(ReceiptEvent),
            _ when string.Equals(eventType, EventTypes.Presence, StringComparison.OrdinalIgnoreCase)
                => typeof(PresenceEvent),
            _ when string.Equals(eventType, EventTypes.ChatPresence, StringComparison.OrdinalIgnoreCase)
                => typeof(ChatPresenceEvent),
            _ when string.Equals(eventType, EventTypes.Connected, StringComparison.OrdinalIgnoreCase)
                => typeof(ConnectedEvent),
            _ when string.Equals(eventType, EventTypes.Disconnected, StringComparison.OrdinalIgnoreCase)
                => typeof(DisconnectedEvent),
            _ when string.Equals(eventType, EventTypes.Qr, StringComparison.OrdinalIgnoreCase)
                => typeof(QrCodeEvent),
            _ when string.Equals(eventType, EventTypes.QrTimeout, StringComparison.OrdinalIgnoreCase)
                => typeof(QrTimeoutEvent),
            _ when string.Equals(eventType, EventTypes.PairSuccess, StringComparison.OrdinalIgnoreCase)
                => typeof(PairSuccessEvent),
            _ when string.Equals(eventType, EventTypes.PairError, StringComparison.OrdinalIgnoreCase)
                => typeof(PairErrorEvent),
            _ when string.Equals(eventType, EventTypes.LoggedOut, StringComparison.OrdinalIgnoreCase)
                => typeof(LoggedOutEvent),
            _ when string.Equals(eventType, EventTypes.ConnectFailure, StringComparison.OrdinalIgnoreCase)
                => typeof(ConnectFailureEvent),
            _ when string.Equals(eventType, EventTypes.ClientOutdated, StringComparison.OrdinalIgnoreCase)
                => typeof(ClientOutdatedEvent),
            _ when string.Equals(eventType, EventTypes.TemporaryBan, StringComparison.OrdinalIgnoreCase)
                => typeof(TemporaryBanEvent),
            _ when string.Equals(eventType, EventTypes.StreamError, StringComparison.OrdinalIgnoreCase)
                => typeof(StreamErrorEvent),
            _ when string.Equals(eventType, EventTypes.StreamReplaced, StringComparison.OrdinalIgnoreCase)
                => typeof(StreamReplacedEvent),
            _ when string.Equals(eventType, EventTypes.KeepAliveTimeout, StringComparison.OrdinalIgnoreCase)
                => typeof(KeepAliveTimeoutEvent),
            _ when string.Equals(eventType, EventTypes.KeepAliveRestored, StringComparison.OrdinalIgnoreCase)
                => typeof(KeepAliveRestoredEvent),
            _ when string.Equals(eventType, EventTypes.CallOffer, StringComparison.OrdinalIgnoreCase)
                => typeof(CallOfferEvent),
            _ when string.Equals(eventType, EventTypes.CallAccept, StringComparison.OrdinalIgnoreCase)
                => typeof(CallAcceptEvent),
            _ when string.Equals(eventType, EventTypes.CallTerminate, StringComparison.OrdinalIgnoreCase)
                => typeof(CallTerminateEvent),
            _ when string.Equals(eventType, EventTypes.CallOfferNotice, StringComparison.OrdinalIgnoreCase)
                => typeof(CallOfferNoticeEvent),
            _ when string.Equals(eventType, EventTypes.CallRelayLatency, StringComparison.OrdinalIgnoreCase)
                => typeof(CallRelayLatencyEvent),
            _ when string.Equals(eventType, EventTypes.GroupInfo, StringComparison.OrdinalIgnoreCase)
                => typeof(GroupInfoEvent),
            _ when string.Equals(eventType, EventTypes.JoinedGroup, StringComparison.OrdinalIgnoreCase)
                => typeof(JoinedGroupEvent),
            _ when string.Equals(eventType, EventTypes.Picture, StringComparison.OrdinalIgnoreCase)
                => typeof(PictureEvent),
            _ when string.Equals(eventType, EventTypes.HistorySync, StringComparison.OrdinalIgnoreCase)
                => typeof(HistorySyncEvent),
            _ when string.Equals(eventType, EventTypes.AppState, StringComparison.OrdinalIgnoreCase)
                => typeof(AppStateEvent),
            _ when string.Equals(eventType, EventTypes.AppStateSyncComplete, StringComparison.OrdinalIgnoreCase)
                => typeof(AppStateSyncCompleteEvent),
            _ when string.Equals(eventType, EventTypes.OfflineSyncCompleted, StringComparison.OrdinalIgnoreCase)
                => typeof(OfflineSyncCompletedEvent),
            _ when string.Equals(eventType, EventTypes.OfflineSyncPreview, StringComparison.OrdinalIgnoreCase)
                => typeof(OfflineSyncPreviewEvent),
            _ when string.Equals(eventType, EventTypes.PrivacySettings, StringComparison.OrdinalIgnoreCase)
                => typeof(PrivacySettingsEvent),
            _ when string.Equals(eventType, EventTypes.PushNameSetting, StringComparison.OrdinalIgnoreCase)
                => typeof(PushNameSettingEvent),
            _ when string.Equals(eventType, EventTypes.BlocklistChange, StringComparison.OrdinalIgnoreCase)
                => typeof(BlocklistChangeEvent),
            _ when string.Equals(eventType, EventTypes.Blocklist, StringComparison.OrdinalIgnoreCase)
                => typeof(BlocklistEvent),
            _ when string.Equals(eventType, EventTypes.IdentityChange, StringComparison.OrdinalIgnoreCase)
                => typeof(IdentityChangeEvent),
            _ when string.Equals(eventType, EventTypes.NewsletterJoin, StringComparison.OrdinalIgnoreCase)
                => typeof(NewsletterJoinEvent),
            _ when string.Equals(eventType, EventTypes.NewsletterLeave, StringComparison.OrdinalIgnoreCase)
                => typeof(NewsletterLeaveEvent),
            _ when string.Equals(eventType, EventTypes.NewsletterMuteChange, StringComparison.OrdinalIgnoreCase)
                => typeof(NewsletterMuteChangeEvent),
            _ when string.Equals(eventType, EventTypes.NewsletterLiveUpdate, StringComparison.OrdinalIgnoreCase)
                => typeof(NewsletterLiveUpdateEvent),
            _ when string.Equals(eventType, EventTypes.MediaRetry, StringComparison.OrdinalIgnoreCase)
                => typeof(MediaRetryEvent),
            _ when string.Equals(eventType, EventTypes.UserAbout, StringComparison.OrdinalIgnoreCase)
                => typeof(UserAboutEvent),
            _ when string.Equals(eventType, EventTypes.FbMessage, StringComparison.OrdinalIgnoreCase)
                => typeof(FbMessageEvent),
            _ => typeof(UnknownEvent)
        };

        // Deserialize the event element to the target type
        var eventObj = (WuzEvent?)JsonSerializer.Deserialize(
            eventElement.GetRawText(),
            targetType,
            options);

        if (eventObj == null)
        {
            return null;
        }

        // Clone the event element to avoid disposed document issues
        // Parse the raw text to create a new independent JsonDocument
        var clonedEventDoc = JsonDocument.Parse(eventElement.GetRawText());

        // Use 'with' expression to set base properties from root
        return eventObj with
        {
            Type = eventType,
            UserId = userId,
            InstanceName = instanceName,
            RawEvent = clonedEventDoc.RootElement
        };
    }

    /// <inheritdoc/>
    public override void Write(
        Utf8JsonWriter writer,
        WuzEvent value,
        JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}

/// <summary>
/// Represents an unknown or unsupported event type.
/// This is used as a fallback when a specific event type is not implemented.
/// </summary>
internal sealed record UnknownEvent : WuzEvent
{
}
