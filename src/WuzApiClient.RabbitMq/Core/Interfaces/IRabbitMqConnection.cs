using System;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace WuzApiClient.RabbitMq.Core.Interfaces;

/// <summary>
/// Manages the RabbitMQ connection.
/// NOTE: This is specifically a connection manager for RabbitMQ.Client,
/// not a transport-agnostic abstraction. The IChannel type is from RabbitMQ.Client.
/// </summary>
public interface IRabbitMqConnection : IDisposable
{
    /// <summary>
    /// Gets whether the connection is open.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Creates a new channel for consuming.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A RabbitMQ.Client IChannel instance.</returns>
    Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to reconnect if disconnected.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>true if reconnection succeeded; otherwise, false.</returns>
    Task<bool> TryReconnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Disposes the connection asynchronously.
    /// Provided for coordination with IHostedService.StopAsync since
    /// IAsyncDisposable is not available in .NET Standard 2.0.
    /// </summary>
    /// <returns>A task representing the async dispose operation.</returns>
    Task DisposeAsync();
}
