namespace WuzApiClient.Common.Results;

/// <summary>
/// Defines error codes for WuzAPI operations.
/// </summary>
public enum WuzApiErrorCode
{
    /// <summary>Unknown error occurred.</summary>
    Unknown = 0,

    /// <summary>Network connection error.</summary>
    NetworkError = 1,

    /// <summary>Request timed out.</summary>
    Timeout = 2,

    /// <summary>Response deserialization failed.</summary>
    DeserializationError = 3,

    /// <summary>Bad request - invalid parameters.</summary>
    BadRequest = 400,

    /// <summary>Unauthorized - invalid token.</summary>
    Unauthorized = 401,

    /// <summary>Forbidden - insufficient permissions.</summary>
    Forbidden = 403,

    /// <summary>Resource not found.</summary>
    NotFound = 404,

    /// <summary>Conflict - resource already exists.</summary>
    Conflict = 409,

    /// <summary>Rate limit exceeded.</summary>
    RateLimitExceeded = 429,

    /// <summary>Internal server error.</summary>
    InternalServerError = 500,

    /// <summary>Session not ready (not connected or not logged in).</summary>
    SessionNotReady = 1000,

    /// <summary>Session already logged in.</summary>
    AlreadyLoggedIn = 1001,

    /// <summary>Invalid phone number format.</summary>
    InvalidPhoneNumber = 1002,

    /// <summary>Invalid file format or size.</summary>
    InvalidFile = 1003,

    /// <summary>Invalid request parameters.</summary>
    InvalidRequest = 1004,

    /// <summary>Unexpected response from API.</summary>
    UnexpectedResponse = 9999
}
