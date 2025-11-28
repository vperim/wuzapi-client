using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WuzApiClient.RabbitMq.HealthChecks;

/// <summary>
/// Extension methods for adding WuzEvents health checks.
/// </summary>
public static class WuzEventsHealthCheckExtensions
{
    /// <summary>
    /// Adds the WuzEvents RabbitMQ health check.
    /// </summary>
    /// <param name="builder">The health checks builder.</param>
    /// <param name="name">The health check name. Default: "wuzevents-rabbitmq".</param>
    /// <param name="tags">Optional tags.</param>
    /// <returns>The builder for chaining.</returns>
    public static IHealthChecksBuilder AddWuzEventsHealthCheck(
        this IHealthChecksBuilder builder,
        string name = "wuzevents-rabbitmq",
        params string[] tags)
    {
        if (builder == null) throw new System.ArgumentNullException(nameof(builder));

        return builder.AddCheck<WuzEventsHealthCheck>(
            name,
            failureStatus: HealthStatus.Unhealthy,
            tags: tags ?? []);
    }
}
