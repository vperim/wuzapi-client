using WuzApiClient.RabbitMq;

namespace WuzApiClient.EventDashboard.Models;

public static class EventCategoryMapper
{
    public static EventCategory MapToCategory(string eventType) => eventType switch
    {
        // Use constants from WuzApiClient.RabbitMq.EventTypes for maintainability
        EventTypes.Message or EventTypes.UndecryptableMessage or EventTypes.FbMessage => EventCategory.Message,
        EventTypes.Receipt => EventCategory.Receipt,
        EventTypes.Presence or EventTypes.ChatPresence => EventCategory.Presence,
        EventTypes.Connected or EventTypes.Disconnected or EventTypes.LoggedOut or EventTypes.ConnectFailure
            or EventTypes.ClientOutdated or EventTypes.TemporaryBan or EventTypes.StreamError
            or EventTypes.StreamReplaced or EventTypes.KeepAliveTimeout or EventTypes.KeepAliveRestored => EventCategory.Connection,
        EventTypes.CallOffer or EventTypes.CallAccept or EventTypes.CallTerminate
            or EventTypes.CallOfferNotice or EventTypes.CallRelayLatency => EventCategory.Call,
        EventTypes.GroupInfo or EventTypes.JoinedGroup => EventCategory.Group,
        _ => EventCategory.System
    };
}
