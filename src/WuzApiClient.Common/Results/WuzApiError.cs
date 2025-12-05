using System;

namespace WuzApiClient.Common.Results;

/// <summary>
/// Represents an error from the WuzAPI service.
/// </summary>
public sealed class WuzApiError : IEquatable<WuzApiError>
{
    /// <summary>
    /// Gets the error code.
    /// </summary>
    public WuzApiErrorCode Code { get; }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the HTTP status code, if applicable.
    /// </summary>
    public int? HttpStatusCode { get; init; }

    /// <summary>
    /// Gets the raw response body, if available.
    /// </summary>
    public string? ResponseBody { get; init; }

    /// <summary>
    /// Initializes a new instance of <see cref="WuzApiError"/>.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    public WuzApiError(WuzApiErrorCode code, string message)
    {
        this.Code = code;
        this.Message = message ?? throw new ArgumentNullException(nameof(message));
    }

    /// <summary>
    /// Creates an error from HTTP status code and response.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="responseBody">The response body.</param>
    /// <returns>A new <see cref="WuzApiError"/> instance.</returns>
    public static WuzApiError FromHttpStatus(int statusCode, string responseBody)
    {
        var code = statusCode switch
        {
            400 => WuzApiErrorCode.BadRequest,
            401 => WuzApiErrorCode.Unauthorized,
            403 => WuzApiErrorCode.Forbidden,
            404 => WuzApiErrorCode.NotFound,
            409 => WuzApiErrorCode.Conflict,
            429 => WuzApiErrorCode.RateLimitExceeded,
            >= 500 and < 600 => WuzApiErrorCode.InternalServerError,
            _ => WuzApiErrorCode.UnexpectedResponse
        };

        var message = ExtractErrorMessage(responseBody) ?? $"HTTP {statusCode}";

        return new WuzApiError(code, message)
        {
            HttpStatusCode = statusCode,
            ResponseBody = responseBody
        };
    }

    /// <summary>
    /// Creates a network error.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <returns>A new <see cref="WuzApiError"/> instance.</returns>
    public static WuzApiError NetworkError(string message) =>
        new(WuzApiErrorCode.NetworkError, message);

    /// <summary>
    /// Creates a timeout error.
    /// </summary>
    /// <returns>A new <see cref="WuzApiError"/> instance.</returns>
    public static WuzApiError Timeout() =>
        new(WuzApiErrorCode.Timeout, "Request timed out");

    /// <summary>
    /// Creates a session not ready error.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <returns>A new <see cref="WuzApiError"/> instance.</returns>
    public static WuzApiError SessionNotReady(string message) =>
        new(WuzApiErrorCode.SessionNotReady, message);

    /// <summary>
    /// Creates an already logged in error.
    /// </summary>
    /// <returns>A new <see cref="WuzApiError"/> instance.</returns>
    public static WuzApiError AlreadyLoggedIn() =>
        new(WuzApiErrorCode.AlreadyLoggedIn, "Session is already logged in");

    /// <summary>
    /// Creates a deserialization error.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <returns>A new <see cref="WuzApiError"/> instance.</returns>
    public static WuzApiError DeserializationError(string message) =>
        new(WuzApiErrorCode.DeserializationError, message);

    /// <summary>
    /// Creates an invalid request error.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <returns>A new <see cref="WuzApiError"/> instance.</returns>
    public static WuzApiError InvalidRequest(string message) =>
        new(WuzApiErrorCode.InvalidRequest, message);

    /// <summary>
    /// Creates an invalid phone number error.
    /// </summary>
    /// <param name="phone">The invalid phone number.</param>
    /// <returns>A new <see cref="WuzApiError"/> instance.</returns>
    public static WuzApiError InvalidPhoneNumber(string phone) =>
        new(WuzApiErrorCode.InvalidPhoneNumber, $"Invalid phone number format: {phone}");

    private static string? ExtractErrorMessage(string? responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
            return null;

        var span = responseBody.AsSpan().Trim();

        if (span.Length == 0 || span[0] != '{')
            return null; // We only try to parse simple JSON objects

        return TryGetJsonStringProperty(span, "error")
               ?? TryGetJsonStringProperty(span, "message");
    }

    private static string? TryGetJsonStringProperty(ReadOnlySpan<char> json, string propertyName)
    {
        // Look for:  "propertyName"
        var pattern = $"\"{propertyName}\"".AsSpan();
        var index = json.IndexOf(pattern, StringComparison.Ordinal);
        if (index < 0)
            return null;

        // Move to after the property name
        json = json[(index + pattern.Length)..];

        // Skip whitespace
        json = SkipWhitespace(json);
        if (json.IsEmpty || json[0] != ':')
            return null;

        // Skip ':' and following whitespace
        json = json[1..];
        json = SkipWhitespace(json);
        if (json.IsEmpty)
            return null;

        // We only handle string values: "something"
        if (json[0] != '"')
            return null;

        // Find closing quote, respecting simple \" escapes
        var i = 1;
        while (i < json.Length)
        {
            var c = json[i];
            if (c == '\\')
            {
                // Skip escaped character: \" or \\ etc.
                i += 2;
                continue;
            }

            if (c == '"')
            {
                var valueSpan = json[1..i];
                // You can optionally unescape \" here; for now we just return raw contents
                return valueSpan.ToString();
            }

            i++;
        }

        return null;
    }

    private static ReadOnlySpan<char> SkipWhitespace(ReadOnlySpan<char> span)
    {
        var i = 0;
        while (i < span.Length && char.IsWhiteSpace(span[i]))
            i++;

        return span[i..];
    }

    /// <inheritdoc/>
    public bool Equals(WuzApiError? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return this.Code == other.Code && this.Message == other.Message;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => this.Equals(obj as WuzApiError);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        unchecked
        {
            return ((int)this.Code * 397) ^ this.Message.GetHashCode();
        }
    }

    /// <inheritdoc/>
    public override string ToString() => $"[{this.Code}] {this.Message}";
}
