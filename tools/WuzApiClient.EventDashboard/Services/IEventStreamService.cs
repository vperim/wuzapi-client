using WuzApiClient.EventDashboard.Models;
using WuzApiClient.RabbitMq.Models;

namespace WuzApiClient.EventDashboard.Services;

public interface IEventStreamService
{
    IReadOnlyList<EventEntry> RecentEvents { get; }
    bool IsConnected { get; }
    string? ConnectionError { get; }
    event Action? OnEventsChanged;
    event Action? OnConnectionStateChanged;
    void AddEvent(WuzEvent evt, string rawJson);
    void AddError(string error, Exception? exception);
    void SetConnectionState(bool isConnected, string? error);
}
