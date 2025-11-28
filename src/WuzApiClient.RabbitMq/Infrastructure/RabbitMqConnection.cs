using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using WuzApiClient.RabbitMq.Configuration;
using WuzApiClient.RabbitMq.Core.Interfaces;

namespace WuzApiClient.RabbitMq.Infrastructure;

/// <summary>
/// Manages the RabbitMQ connection with thread-safe connection state management.
/// Updated for RabbitMQ.Client 7.x async-first API.
/// </summary>
public sealed class RabbitMqConnection : IRabbitMqConnection
{
    private readonly WuzEventOptions options;
    private readonly ILogger<RabbitMqConnection> logger;
    private readonly ConnectionFactory connectionFactory;
    private readonly SemaphoreSlim connectionLock;
    private IConnection? connection;
    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqConnection"/> class.
    /// </summary>
    /// <param name="options">The event consumer options.</param>
    /// <param name="logger">The logger instance.</param>
    public RabbitMqConnection(
        IOptions<WuzEventOptions> options,
        ILogger<RabbitMqConnection> logger)
    {
        this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.connectionLock = new SemaphoreSlim(1, 1);

        // Parse connection string to create ConnectionFactory
        this.connectionFactory = new ConnectionFactory
        {
            Uri = new Uri(this.options.ConnectionString),
        };

        this.logger.LogDebug(
            "RabbitMqConnection initialized with URI: {Uri}",
            this.connectionFactory.Uri.GetLeftPart(UriPartial.Authority));
    }

    /// <inheritdoc/>
    public bool IsConnected => this.connection is { IsOpen: true };

    /// <inheritdoc/>
    public async Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken = default)
    {
        this.ThrowIfDisposed();

        // Lazy create connection on first CreateChannelAsync() call
        await this.EnsureConnectedAsync(cancellationToken).ConfigureAwait(false);

        if (this.connection is not { IsOpen: true })
        {
            throw new InvalidOperationException("RabbitMQ connection is not open.");
        }

        var channel = await this.connection.CreateChannelAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        this.logger.LogDebug("Created new RabbitMQ channel.");
        return channel;
    }

    /// <inheritdoc/>
    public async Task<bool> TryReconnectAsync(CancellationToken cancellationToken = default)
    {
        this.ThrowIfDisposed();

        await this.connectionLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            // If already connected, return success
            if (this.IsConnected)
            {
                this.logger.LogDebug("Already connected, skipping reconnection.");
                return true;
            }

            // Close existing connection if it exists
            await this.CloseConnectionAsync().ConfigureAwait(false);

            var baseDelay = this.options.ReconnectDelay;
            var maxAttempts = this.options.MaxReconnectAttempts;

            for (var attempt = 0; attempt < maxAttempts; attempt++)
            {
                try
                {
                    this.logger.LogInformation(
                        "Attempting to reconnect to RabbitMQ (attempt {Attempt}/{MaxAttempts})...",
                        attempt + 1,
                        maxAttempts);

                    this.connection = await this.connectionFactory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);
                    this.SubscribeToConnectionEvents();

                    this.logger.LogInformation("Successfully reconnected to RabbitMQ.");
                    return true;
                }
                catch (Exception ex)
                {
                    this.logger.LogWarning(
                        ex,
                        "Reconnection attempt {Attempt}/{MaxAttempts} failed: {Error}",
                        attempt + 1,
                        maxAttempts,
                        ex.Message);

                    // If this was the last attempt, return failure
                    if (attempt == maxAttempts - 1)
                    {
                        this.logger.LogError(
                            "All reconnection attempts exhausted. Failed to reconnect to RabbitMQ.");
                        return false;
                    }

                    // Calculate exponential backoff delay: baseDelay * 2^attempt, capped at 60 seconds
                    var delay = TimeSpan.FromMilliseconds(
                        Math.Min(
                            baseDelay.TotalMilliseconds * Math.Pow(2, attempt),
                            60000));

                    this.logger.LogDebug(
                        "Waiting {Delay} before next reconnection attempt...",
                        delay);

                    await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                }
            }

            return false;
        }
        finally
        {
            this.connectionLock.Release();
        }
    }

    /// <inheritdoc/>
    public async Task DisposeAsync()
    {
        if (this.disposed)
        {
            return;
        }

        await this.connectionLock.WaitAsync().ConfigureAwait(false);
        try
        {
            this.logger.LogInformation("Disposing RabbitMQ connection...");

            await this.CloseConnectionAsync().ConfigureAwait(false);
            this.connectionLock.Dispose();

            this.disposed = true;

            this.logger.LogInformation("RabbitMQ connection disposed.");
        }
        finally
        {
            if (!this.disposed)
            {
                this.connectionLock.Release();
            }
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.DisposeAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Ensures a connection exists, creating it lazily if needed.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task EnsureConnectedAsync(CancellationToken cancellationToken = default)
    {
        if (this.IsConnected)
        {
            return;
        }

        await this.connectionLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            // Double-check after acquiring lock
            if (this.IsConnected)
            {
                return;
            }

            this.logger.LogInformation("Creating RabbitMQ connection...");

            this.connection = await this.connectionFactory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);
            this.SubscribeToConnectionEvents();

            this.logger.LogInformation("RabbitMQ connection established.");
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to create RabbitMQ connection: {Error}", ex.Message);
            throw;
        }
        finally
        {
            this.connectionLock.Release();
        }
    }

    /// <summary>
    /// Subscribes to connection events for logging and monitoring.
    /// </summary>
    private void SubscribeToConnectionEvents()
    {
        if (this.connection == null)
        {
            return;
        }

        this.connection.ConnectionShutdownAsync += this.OnConnectionShutdownAsync;
    }

    /// <summary>
    /// Handles connection shutdown events.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The shutdown event arguments.</param>
    private Task OnConnectionShutdownAsync(object? sender, ShutdownEventArgs e)
    {
        if (this.disposed)
        {
            return Task.CompletedTask;
        }

        this.logger.LogWarning(
            "RabbitMQ connection shutdown: {ReplyCode} - {ReplyText}",
            e.ReplyCode,
            e.ReplyText);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Closes and disposes the current connection.
    /// </summary>
    private async Task CloseConnectionAsync()
    {
        if (this.connection != null)
        {
            try
            {
                if (this.connection.IsOpen)
                {
                    await this.connection.CloseAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogWarning(ex, "Error closing RabbitMQ connection: {Error}", ex.Message);
            }
            finally
            {
                this.connection.Dispose();
                this.connection = null;
            }
        }
    }

    /// <summary>
    /// Throws an exception if the instance has been disposed.
    /// </summary>
    private void ThrowIfDisposed()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException(nameof(RabbitMqConnection));
        }
    }
}
