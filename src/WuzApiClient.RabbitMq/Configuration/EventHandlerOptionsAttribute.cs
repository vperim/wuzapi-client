using System;

namespace WuzApiClient.RabbitMq.Configuration;

/// <summary>
/// Configures error handling behavior for an event handler.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class EventHandlerOptionsAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the error behavior. Default: AcknowledgeOnError.
    /// </summary>
    public ErrorBehavior ErrorBehavior { get; set; } = ErrorBehavior.AcknowledgeOnError;

    /// <summary>
    /// Gets or sets the maximum retry count. Default: 0.
    /// </summary>
    public int MaxRetries { get; set; } = 0;
}
