using System;

namespace WuzApiClient.Exceptions;

/// <summary>
/// Exception thrown when WuzAPI client configuration is invalid.
/// </summary>
public sealed class WuzApiConfigurationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WuzApiConfigurationException"/> class.
    /// </summary>
    public WuzApiConfigurationException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WuzApiConfigurationException"/> class
    /// with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public WuzApiConfigurationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WuzApiConfigurationException"/> class
    /// with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that caused this exception.</param>
    public WuzApiConfigurationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
