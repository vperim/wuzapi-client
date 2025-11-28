using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WuzApiClient.RabbitMq.Core.Interfaces;
using WuzApiClient.RabbitMq.Models;

namespace WuzApiClient.RabbitMq.Infrastructure;

/// <summary>
/// Default error handler that logs errors using ILogger.
/// </summary>
public sealed class LoggingEventErrorHandler : IEventErrorHandler
{
    private readonly ILogger<LoggingEventErrorHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingEventErrorHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <exception cref="ArgumentNullException">Thrown when logger is null.</exception>
    public LoggingEventErrorHandler(ILogger<LoggingEventErrorHandler> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public Task HandleErrorAsync(
        WuzEvent evt,
        Exception exception,
        CancellationToken cancellationToken)
    {
        this.logger.LogError(
            exception,
            "Error handling event {EventType} for user {UserId}/{InstanceName}: {Message}",
            evt.Type,
            evt.UserId,
            evt.InstanceName,
            exception.Message);

        return Task.CompletedTask;
    }
}
