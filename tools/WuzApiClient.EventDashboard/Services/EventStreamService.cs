using Microsoft.Extensions.Options;
using WuzApiClient.EventDashboard.Models;
using WuzApiClient.EventDashboard.Models.Metadata;
using WuzApiClient.RabbitMq.Core;
using WuzApiClient.RabbitMq.Core.Interfaces;
using System.Text.Json;
using WuzApiClient.RabbitMq.Models.Wuz;

namespace WuzApiClient.EventDashboard.Services;

public sealed class EventStreamService : IEventStreamService, IDisposable
{
    private readonly object gate = new();
    private readonly List<EventEntry> events = [];
    private readonly int maxSize;
    private readonly IEventConsumer consumer;
    private readonly ILogger<EventStreamService> logger;

    private bool isConnected;
    private string? connectionError;

    public EventStreamService(
        IEventConsumer consumer,
        IOptions<DashboardOptions> options,
        ILogger<EventStreamService> logger)
    {
        this.consumer = consumer;
        this.logger = logger;
        maxSize = options.Value.MaxEventBufferSize;

        // Subscribe to connection state changes
        consumer.ConnectionStateChanged += OnConnectionStateChangedInternal;

        // Initialize with current connection state (consumer may have started before we subscribed)
        isConnected = consumer.IsConnected;
        connectionError = isConnected ? null : "Waiting for connection...";
    }

    public IReadOnlyList<EventEntry> RecentEvents
    {
        get
        {
            lock (gate)
            {
                // Snapshot copy – safe for concurrent UI reads
                return events.ToList();
            }
        }
    }

    // Thread-safe property reads (all shared state accessed under lock)
    public bool IsConnected
    {
        get { lock (gate) { return isConnected; } }
    }

    public string? ConnectionError
    {
        get { lock (gate) { return connectionError; } }
    }

    public event Action? OnEventsChanged;
    public event Action? OnConnectionStateChanged;

    public void AddEvent(IWuzEventEnvelope envelope, EventMetadata metadata)
    {
        var entry = CreateEntry(envelope, metadata);

        lock (gate)
        {
            events.Add(entry);

            if (events.Count > maxSize)
            {
                var excess = events.Count - maxSize;
                events.RemoveRange(0, excess); // drop oldest
            }
        }

        OnEventsChanged?.Invoke();
    }

    public void AddError(string error, Exception? exception)
    {
        var entry = new EventEntry
        {
            Id = Guid.NewGuid().ToString(),
            Timestamp = DateTimeOffset.UtcNow,
            EventType = "Error",
            Category = EventCategory.System,
            UserId = string.Empty,
            InstanceName = string.Empty,
            Metadata = new SystemMetadata
            {
                Category = EventCategory.System,
                SystemEvent = "Error",
                Details = exception != null ? $"{error}: {exception.Message}" : error,
                IsSyncComplete = false
            },
            Event = null!,
            RawJson = "{}",
            Error = exception != null ? $"{error}: {exception.Message}" : error
        };

        lock (gate)
        {
            events.Add(entry);

            if (events.Count > maxSize)
            {
                var excess = events.Count - maxSize;
                events.RemoveRange(0, excess); // drop oldest
            }
        }

        OnEventsChanged?.Invoke();
    }

    public void SetConnectionState(bool connected, string? error)
    {
        lock (gate)
        {
            isConnected = connected;
            connectionError = error;
        }
        OnConnectionStateChanged?.Invoke();
    }

    private void OnConnectionStateChangedInternal(object? sender, ConnectionStateChangedEventArgs e)
    {
        SetConnectionState(e.IsConnected, e.Reason ?? e.Exception?.Message);
    }

    public void Dispose()
    {
        consumer.ConnectionStateChanged -= OnConnectionStateChangedInternal;
    }

    private static readonly JsonSerializerOptions PrettyPrintOptions = new()
    {
        WriteIndented = true
    };

    private EventEntry CreateEntry(IWuzEventEnvelope envelope, EventMetadata metadata)
    {
        return new EventEntry
        {
            Id = Guid.NewGuid().ToString(),
            Timestamp = envelope.ReceivedAt,
            EventType = envelope.Metadata.WaEventMetadata.Type,
            Category = metadata.Category,
            UserId = envelope.Metadata.WuzEnvelope.UserId,
            InstanceName = envelope.Metadata.WuzEnvelope.InstanceName,
            Metadata = metadata,
            Event = envelope,
            RawJson = FormatJson(envelope.Metadata.WuzEnvelope.JsonData)
        };
    }

    private static string FormatJson(string json)
    {
        try
        {
            // Parse and re-serialize with indentation for display
            using var document = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(document.RootElement, PrettyPrintOptions);
        }
        catch
        {
            return json; // Return original if parsing fails
        }
    }
}
