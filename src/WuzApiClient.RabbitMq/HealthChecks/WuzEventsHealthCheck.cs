using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WuzApiClient.RabbitMq.Core.Interfaces;

namespace WuzApiClient.RabbitMq.HealthChecks;

/// <summary>
/// Health check for RabbitMQ connection status.
/// </summary>
public sealed class WuzEventsHealthCheck : IHealthCheck
{
    private readonly IRabbitMqConnection connection;

    /// <summary>
    /// Initializes a new instance of the WuzEventsHealthCheck class.
    /// </summary>
    public WuzEventsHealthCheck(IRabbitMqConnection connection)
    {
        this.connection = connection ?? throw new System.ArgumentNullException(nameof(connection));
    }

    /// <inheritdoc/>
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var result = this.connection.IsConnected
            ? HealthCheckResult.Healthy("RabbitMQ connected")
            : HealthCheckResult.Unhealthy("RabbitMQ disconnected");

        return Task.FromResult(result);
    }
}
