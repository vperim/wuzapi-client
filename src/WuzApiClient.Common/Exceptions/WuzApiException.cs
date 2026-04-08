using System;
using WuzApiClient.Common.Results;

namespace WuzApiClient.Common.Exceptions;

/// <summary>
/// Exception that preserves WuzApiError context for scenarios
/// where exception-based error handling is preferred over Result pattern.
/// </summary>
public sealed class WuzApiException : Exception
{
    /// <summary>
    /// Gets the underlying WuzApi error with code and details.
    /// </summary>
    public WuzApiError Error { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="WuzApiException"/>.
    /// </summary>
    /// <param name="error">The WuzApi error.</param>
    public WuzApiException(WuzApiError error)
        : base(error.Message)
    {
        this.Error = error ?? throw new ArgumentNullException(nameof(error));
    }

    /// <summary>
    /// Initializes a new instance of <see cref="WuzApiException"/> with an inner exception.
    /// </summary>
    /// <param name="error">The WuzApi error.</param>
    /// <param name="innerException">The inner exception.</param>
    public WuzApiException(WuzApiError error, Exception innerException)
        : base(error.Message, innerException)
    {
        this.Error = error ?? throw new ArgumentNullException(nameof(error));
    }
}
