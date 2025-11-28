using System;

namespace WuzApiClient.RabbitMq.Configuration;

/// <summary>
/// Exception thrown when WuzEvents configuration is invalid.
/// </summary>
public sealed class WuzEventsConfigurationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WuzEventsConfigurationException"/> class
    /// with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public WuzEventsConfigurationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WuzEventsConfigurationException"/> class
    /// with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that caused this exception.</param>
    public WuzEventsConfigurationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
