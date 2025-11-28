using System;
using Microsoft.Extensions.Hosting;

namespace WuzApiClient.RabbitMq.Core.Interfaces;

/// <summary>
/// Consumes events from RabbitMQ and dispatches to handlers.
/// </summary>
public interface IEventConsumer : IHostedService
{
    /// <summary>
    /// Gets whether the consumer is currently connected to RabbitMQ.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Raised when the connection state changes.
    /// </summary>
    event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;
}
