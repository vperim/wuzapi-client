// ReSharper disable InconsistentNaming
namespace WuzApiClient.Common.Enums;

/// <summary>
/// Reason codes for connection failures and logout events.
/// Values correspond to whatsmeow's ConnectFailureReason enum.
/// Source: whatsmeow/types/events/events.go
/// </summary>
public enum ConnectFailureReason
{
    /// <summary>Unknown or unrecognized reason code.</summary>
    Unknown = 0,

    /// <summary>Generic connection failure.</summary>
    Generic = 400,

    /// <summary>User was logged out from WhatsApp.</summary>
    LoggedOut = 401,

    /// <summary>Account is temporarily banned.</summary>
    TempBanned = 402,

    /// <summary>Main device (phone) is no longer available.</summary>
    MainDeviceGone = 403,

    /// <summary>Client version is outdated and needs update.</summary>
    ClientOutdated = 405,

    /// <summary>Unknown logout reason.</summary>
    UnknownLogout = 406,

    /// <summary>Bad user agent string.</summary>
    BadUserAgent = 409,

    /// <summary>Client Access Token has expired.</summary>
    CATExpired = 413,

    /// <summary>Client Access Token is invalid.</summary>
    CATInvalid = 414,

    /// <summary>Resource not found.</summary>
    NotFound = 415,

    /// <summary>Client is unknown to the server.</summary>
    ClientUnknown = 418,

    /// <summary>Internal server error.</summary>
    InternalServerError = 500,

    /// <summary>Experimental feature error.</summary>
    Experimental = 501,

    /// <summary>Service is temporarily unavailable.</summary>
    ServiceUnavailable = 503
}
