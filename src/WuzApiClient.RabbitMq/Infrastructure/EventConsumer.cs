using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using WuzApiClient.RabbitMq.Configuration;
using WuzApiClient.RabbitMq.Core;
using WuzApiClient.RabbitMq.Core.Interfaces;
using WuzApiClient.RabbitMq.Models;
using WuzApiClient.RabbitMq.Serialization;

namespace WuzApiClient.RabbitMq.Infrastructure;

/// <summary>
/// Consumes events from RabbitMQ and dispatches them to registered handlers.
/// Updated for RabbitMQ.Client 7.x async-first API.
/// </summary>
public sealed class EventConsumer : IEventConsumer
{
    private readonly IRabbitMqConnection connection;
    private readonly IEventDispatcher dispatcher;
    private readonly WuzEventOptions options;
    private readonly ILogger<EventConsumer> logger;
    private readonly SemaphoreSlim concurrencySemaphore;
    private CancellationTokenSource? cancellationTokenSource;
    private IChannel? channel;
    private string? consumerTag;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventConsumer"/> class.
    /// </summary>
    /// <param name="connection">The RabbitMQ connection manager.</param>
    /// <param name="dispatcher">The event dispatcher.</param>
    /// <param name="options">The configuration options.</param>
    /// <param name="logger">The logger.</param>
    public EventConsumer(
        IRabbitMqConnection connection,
        IEventDispatcher dispatcher,
        IOptions<WuzEventOptions> options,
        ILogger<EventConsumer> logger)
    {
        this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.concurrencySemaphore = new SemaphoreSlim(this.options.MaxConcurrentMessages);
    }

    /// <inheritdoc/>
    public bool IsConnected => this.connection.IsConnected;

    /// <inheritdoc/>
    public event EventHandler<ConnectionStateChangedEventArgs>? ConnectionStateChanged;

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        this.logger.LogInformation(
            "Starting event consumer for queue '{QueueName}'",
            this.options.QueueName);

        this.cancellationTokenSource = new CancellationTokenSource();

        try
        {
            // Create channel from connection (async in v7)
            this.channel = await this.connection.CreateChannelAsync(cancellationToken).ConfigureAwait(false);

            // Declare queue (creates if doesn't exist) - async in v7
            await this.channel.QueueDeclareAsync(
                queue: this.options.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            // Set BasicQos with options.PrefetchCount - async in v7
            await this.channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: this.options.PrefetchCount,
                global: false,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            // Create AsyncEventingBasicConsumer
            var consumer = new AsyncEventingBasicConsumer(this.channel);

            // Subscribe to consumer.Received event
            consumer.ReceivedAsync += this.OnMessageReceivedAsync;

            // Call BasicConsume with options.QueueName, options.AutoAck, consumer tag - async in v7
            this.consumerTag = await this.channel.BasicConsumeAsync(
                queue: this.options.QueueName,
                autoAck: this.options.AutoAck,
                consumerTag: $"{this.options.ConsumerTagPrefix}-{Guid.NewGuid():N}",
                consumer: consumer,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            this.logger.LogInformation(
                "Event consumer started with tag '{ConsumerTag}' (prefetch: {PrefetchCount}, maxConcurrency: {MaxConcurrentMessages})",
                this.consumerTag,
                this.options.PrefetchCount,
                this.options.MaxConcurrentMessages);

            this.RaiseConnectionStateChanged(true, "Consumer started");
        }
        catch (Exception ex)
        {
            this.logger.LogError(
                ex,
                "Failed to start event consumer");

            this.RaiseConnectionStateChanged(false, "Failed to start consumer", ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Stopping event consumer");

        try
        {
            // Cancel consuming
            this.cancellationTokenSource?.Cancel();

            if (this.channel is { IsOpen: true } && this.consumerTag != null)
            {
                try
                {
                    await this.channel.BasicCancelAsync(this.consumerTag, cancellationToken: cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    this.logger.LogWarning(
                        ex,
                        "Error cancelling consumer tag '{ConsumerTag}'",
                        this.consumerTag);
                }
            }

            // Close channel - async in v7
            if (this.channel != null)
            {
                try
                {
                    await this.channel.CloseAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    this.logger.LogWarning(
                        ex,
                        "Error closing channel");
                }

                this.channel.Dispose();
                this.channel = null;
            }

            // Dispose connection asynchronously
            await this.connection.DisposeAsync().ConfigureAwait(false);

            // Dispose resources
            this.cancellationTokenSource?.Dispose();
            this.cancellationTokenSource = null;

            this.logger.LogInformation("Event consumer stopped");

            this.RaiseConnectionStateChanged(false, "Consumer stopped");
        }
        catch (Exception ex)
        {
            this.logger.LogError(
                ex,
                "Error during event consumer shutdown");
            throw;
        }
    }

    /// <summary>
    /// Handles incoming RabbitMQ messages.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="eventArgs">The message event arguments.</param>
    /// <returns>A task representing the async operation.</returns>
    private async Task OnMessageReceivedAsync(object? sender, BasicDeliverEventArgs eventArgs)
    {
        var ct = this.cancellationTokenSource?.Token ?? CancellationToken.None;

        // Use SemaphoreSlim for concurrency control
        await this.concurrencySemaphore.WaitAsync(ct).ConfigureAwait(false);
        try
        {
            await this.ProcessMessageAsync(eventArgs, ct).ConfigureAwait(false);
        }
        finally
        {
            this.concurrencySemaphore.Release();
        }
    }

    /// <summary>
    /// Processes a single message.
    /// </summary>
    /// <param name="eventArgs">The message event arguments.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the async operation.</returns>
    private async Task ProcessMessageAsync(BasicDeliverEventArgs eventArgs, CancellationToken ct)
    {
        WuzEvent? evt = null;

        try
        {
            // Deserialize message body - RabbitMQ messages are already in the correct format
            var messageBody = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

            this.logger.LogDebug(
                "Received message with delivery tag {DeliveryTag}",
                eventArgs.DeliveryTag);

            // Deserialize directly to WuzEvent using our custom converter
            evt = JsonSerializer.Deserialize<WuzEvent>(
                messageBody,
                WuzEventJsonSerializerOptions.Default);

            if (evt == null)
            {
                this.logger.LogWarning(
                    "Failed to deserialize message with delivery tag {DeliveryTag}. Raw message (truncated): {RawMessage}",
                    eventArgs.DeliveryTag,
                    TruncateForLogging(messageBody, 500));

                await this.NackMessageAsync(eventArgs.DeliveryTag, requeue: false, ct).ConfigureAwait(false);
                return;
            }

            // Call dispatcher.DispatchAsync()
            var result = await this.dispatcher.DispatchAsync(evt, ct).ConfigureAwait(false);

            // Handle ack/nack based on dispatch result and AutoAck setting
            if (!this.options.AutoAck)
            {
                if (result.IsSuccess)
                {
                    await this.AckMessageAsync(eventArgs.DeliveryTag, ct).ConfigureAwait(false);
                }
                else
                {
                    this.logger.LogError(
                        "Failed to dispatch event of type '{EventType}': {Error}",
                        evt.Type,
                        result.Error);

                    // Nack message on failure (requeue based on ErrorBehavior - for now default to requeue=false)
                    await this.NackMessageAsync(eventArgs.DeliveryTag, requeue: false, ct).ConfigureAwait(false);
                }
            }
        }
        catch (JsonException ex)
        {
            // Log with raw message for debugging schema mismatches
            var rawMessage = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            this.logger.LogError(
                ex,
                "JSON deserialization error for message with delivery tag {DeliveryTag}. Raw message (truncated): {RawMessage}",
                eventArgs.DeliveryTag,
                TruncateForLogging(rawMessage, 500));

            // Don't requeue messages that can't be deserialized
            await this.NackMessageAsync(eventArgs.DeliveryTag, requeue: false, ct).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            this.logger.LogInformation(
                "Message processing cancelled for delivery tag {DeliveryTag}",
                eventArgs.DeliveryTag);

            // Requeue cancelled messages
            await this.NackMessageAsync(eventArgs.DeliveryTag, requeue: true, ct).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this.logger.LogError(
                ex,
                "Unexpected error processing message with delivery tag {DeliveryTag} (EventType: {EventType})",
                eventArgs.DeliveryTag,
                evt?.Type ?? "unknown");

            // Nack message on failure (requeue based on ErrorBehavior - for now default to requeue=false)
            await this.NackMessageAsync(eventArgs.DeliveryTag, requeue: false, ct).ConfigureAwait(false);

            // Raise ConnectionStateChanged on connection changes
            if (ex is RabbitMQ.Client.Exceptions.AlreadyClosedException ||
                ex is RabbitMQ.Client.Exceptions.BrokerUnreachableException)
            {
                this.RaiseConnectionStateChanged(false, "Connection lost during message processing", ex);
            }
        }
    }

    /// <summary>
    /// Acknowledges a message asynchronously.
    /// </summary>
    /// <param name="deliveryTag">The delivery tag.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task AckMessageAsync(ulong deliveryTag, CancellationToken cancellationToken = default)
    {
        if (this.channel is not { IsOpen: true })
        {
            this.logger.LogWarning(
                "Cannot acknowledge message {DeliveryTag}: channel is not open",
                deliveryTag);
            return;
        }

        try
        {
            await this.channel.BasicAckAsync(deliveryTag, multiple: false, cancellationToken).ConfigureAwait(false);

            this.logger.LogDebug(
                "Acknowledged message {DeliveryTag}",
                deliveryTag);
        }
        catch (Exception ex)
        {
            this.logger.LogError(
                ex,
                "Error acknowledging message {DeliveryTag}",
                deliveryTag);
        }
    }

    /// <summary>
    /// Negatively acknowledges a message asynchronously.
    /// </summary>
    /// <param name="deliveryTag">The delivery tag.</param>
    /// <param name="requeue">Whether to requeue the message.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task NackMessageAsync(ulong deliveryTag, bool requeue, CancellationToken cancellationToken = default)
    {
        if (this.channel is not { IsOpen: true })
        {
            this.logger.LogWarning(
                "Cannot nack message {DeliveryTag}: channel is not open",
                deliveryTag);
            return;
        }

        try
        {
            await this.channel.BasicNackAsync(deliveryTag, multiple: false, requeue: requeue, cancellationToken).ConfigureAwait(false);

            this.logger.LogDebug(
                "Nacked message {DeliveryTag} (requeue: {Requeue})",
                deliveryTag,
                requeue);
        }
        catch (Exception ex)
        {
            this.logger.LogError(
                ex,
                "Error nacking message {DeliveryTag}",
                deliveryTag);
        }
    }

    /// <summary>
    /// Raises the ConnectionStateChanged event.
    /// </summary>
    /// <param name="isConnected">Whether the consumer is connected.</param>
    /// <param name="reason">The reason for the state change.</param>
    /// <param name="exception">The exception that caused the state change.</param>
    private void RaiseConnectionStateChanged(bool isConnected, string? reason = null, Exception? exception = null)
    {
        try
        {
            var eventArgs = new ConnectionStateChangedEventArgs(isConnected, reason, exception);
            this.ConnectionStateChanged?.Invoke(this, eventArgs);
        }
        catch (Exception ex)
        {
            this.logger.LogError(
                ex,
                "Error raising ConnectionStateChanged event");
        }
    }

    /// <summary>
    /// Truncates a string for safe logging, avoiding massive log entries.
    /// </summary>
    /// <param name="value">The string to truncate.</param>
    /// <param name="maxLength">Maximum length before truncation.</param>
    /// <returns>The truncated string with indicator if truncated.</returns>
    private static string TruncateForLogging(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
        {
            return value;
        }

        return value[..maxLength] + "... [TRUNCATED]";
    }
}
