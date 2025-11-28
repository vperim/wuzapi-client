namespace WuzApiClient.RabbitMq.Configuration;

/// <summary>
/// Defines how errors during event handling are processed.
/// </summary>
public enum ErrorBehavior
{
    /// <summary>
    /// Acknowledge message even if handler fails (fire-and-forget).
    /// </summary>
    AcknowledgeOnError,

    /// <summary>
    /// Reject and requeue message on handler failure.
    /// </summary>
    RequeueOnError,

    /// <summary>
    /// Reject without requeue (dead-letter if configured).
    /// </summary>
    DeadLetterOnError,
}
