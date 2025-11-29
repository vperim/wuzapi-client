using WuzApiClient.RabbitMq;
using WuzApiClient.RabbitMq.Core.Interfaces;
using WuzApiClient.RabbitMq.Models;

namespace WuzApiClient.EventDashboard.Services;

public class DashboardEventHandler : IEventHandler
{
    private readonly IEventStreamService eventStream;

    public DashboardEventHandler(IEventStreamService eventStream)
    {
        this.eventStream = eventStream;
    }

    public IReadOnlyCollection<string> EventTypes => RabbitMq.EventTypes.All;

    public Task HandleAsync(WuzEvent evt, CancellationToken cancellationToken = default)
    {
        var rawJson = evt.RawEvent?.GetRawText() ?? "{}";
        eventStream.AddEvent(evt, rawJson);
        return Task.CompletedTask;
    }
}
