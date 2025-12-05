using WuzApiClient.EventDashboard.Models;
using WuzApiClient.EventDashboard.Models.Metadata;
using WuzApiClient.RabbitMq.Models.Wuz;

namespace WuzApiClient.EventDashboard.Services;

public interface IEventStreamService
{
    IReadOnlyList<EventEntry> RecentEvents { get; }
    bool IsConnected { get; }
    string? ConnectionError { get; }
    event Action? OnEventsChanged;
    event Action? OnConnectionStateChanged;
    void AddEvent(IWuzEventEnvelope envelope, EventMetadata metadata);
    void AddError(string error, Exception? exception);
    void SetConnectionState(bool isConnected, string? error);
}
