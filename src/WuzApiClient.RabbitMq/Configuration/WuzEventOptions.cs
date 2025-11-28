using System;
using System.Collections.Generic;

namespace WuzApiClient.RabbitMq.Configuration;

/// <summary>
/// Configuration options for the event consumer.
/// </summary>
public sealed class WuzEventOptions
{
    /// <summary>
    /// Gets or sets the RabbitMQ connection string (amqp://user:pass@host:port/vhost).
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the queue name to consume from. Default: "whatsapp_events".
    /// </summary>
    public string QueueName { get; set; } = "whatsapp_events";

    /// <summary>
    /// Gets or sets the consumer tag prefix. Default: "wuzapi-consumer".
    /// </summary>
    public string ConsumerTagPrefix { get; set; } = "wuzapi-consumer";

    /// <summary>
    /// Gets or sets the prefetch count for the consumer. Default: 10.
    /// </summary>
    public ushort PrefetchCount { get; set; } = 10;

    /// <summary>
    /// Gets or sets whether to auto-acknowledge messages. Default: false.
    /// </summary>
    public bool AutoAck { get; set; } = false;

    /// <summary>
    /// Gets or sets the maximum reconnection attempts. Default: 10.
    /// </summary>
    public int MaxReconnectAttempts { get; set; } = 10;

    /// <summary>
    /// Gets or sets the base delay between reconnection attempts. Default: 3 seconds.
    /// </summary>
    public TimeSpan ReconnectDelay { get; set; } = TimeSpan.FromSeconds(3);

    /// <summary>
    /// Gets or sets the maximum number of messages to process concurrently. Default: Environment.ProcessorCount.
    /// When set to 1, messages are processed sequentially.
    /// NOTE: Message ordering is NOT guaranteed when concurrency &gt; 1.
    /// </summary>
    public int MaxConcurrentMessages { get; set; } = Environment.ProcessorCount;

    /// <summary>
    /// Gets or sets the event types to subscribe to. Empty = all events.
    /// NOTE: Uses HashSet for configuration binding compatibility.
    /// </summary>
    public HashSet<string> SubscribedEventTypes { get; set; } = [];

    /// <summary>
    /// Gets or sets the user IDs to filter events for. Empty = all users.
    /// NOTE: Uses HashSet for configuration binding compatibility.
    /// </summary>
    public HashSet<string> FilterUserIds { get; set; } = [];

    /// <summary>
    /// Gets or sets the instance names to filter events for. Empty = all instances.
    /// NOTE: Uses HashSet for configuration binding compatibility.
    /// </summary>
    public HashSet<string> FilterInstanceNames { get; set; } = [];

    /// <summary>
    /// Validates the configuration options.
    /// </summary>
    /// <exception cref="WuzEventsConfigurationException">Thrown when configuration is invalid.</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(this.ConnectionString))
        {
            throw new WuzEventsConfigurationException(
                $"{nameof(this.ConnectionString)} is required.");
        }

        if (string.IsNullOrWhiteSpace(this.QueueName))
        {
            throw new WuzEventsConfigurationException(
                $"{nameof(this.QueueName)} is required.");
        }

        if (this.PrefetchCount == 0)
        {
            throw new WuzEventsConfigurationException(
                $"{nameof(this.PrefetchCount)} must be greater than 0.");
        }

        if (this.MaxReconnectAttempts < 0)
        {
            throw new WuzEventsConfigurationException(
                $"{nameof(this.MaxReconnectAttempts)} cannot be negative.");
        }

        if (this.ReconnectDelay < TimeSpan.Zero)
        {
            throw new WuzEventsConfigurationException(
                $"{nameof(this.ReconnectDelay)} cannot be negative.");
        }

        if (this.MaxConcurrentMessages <= 0)
        {
            throw new WuzEventsConfigurationException(
                $"{nameof(this.MaxConcurrentMessages)} must be greater than 0.");
        }
    }
}
