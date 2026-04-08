namespace WuzApiClient.Models.Common;

/// <summary>
/// Defines the types of interactive buttons supported by WhatsApp.
/// </summary>
public enum ButtonType
{
    /// <summary>
    /// A quick reply button.
    /// </summary>
    Reply,

    /// <summary>
    /// A call-to-action button that opens a URL.
    /// </summary>
    CtaUrl,

    /// <summary>
    /// A call-to-action button that initiates a phone call.
    /// </summary>
    CtaCall,

    /// <summary>
    /// A button that copies a code to the clipboard.
    /// </summary>
    Copy
}
