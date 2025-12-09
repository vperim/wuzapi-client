namespace WuzApiClient.Common.Enums;

/// <summary>
/// Type of message receipt from WhatsApp.
/// Values correspond to whatsmeow's ReceiptType constants.
/// Source: go.mau.fi/whatsmeow/types
/// </summary>
/// <remarks>
/// Note: wuzapi only sends webhooks for Read, ReadSelf, and Delivered receipts.
/// Other receipt types are discarded by wuzapi before reaching the webhook.
/// </remarks>
public enum ReceiptType
{
    /// <summary>
    /// Unknown or unrecognized receipt type.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Message was delivered to the recipient's device (but user may not have noticed).
    /// Corresponds to whatsmeow empty string "".
    /// </summary>
    Delivered,

    /// <summary>
    /// Sent by your other devices when a message you sent is delivered to them.
    /// Corresponds to whatsmeow "sender".
    /// </summary>
    Sender,

    /// <summary>
    /// Message was delivered but decryption failed.
    /// Corresponds to whatsmeow "retry".
    /// </summary>
    Retry,

    /// <summary>
    /// User opened the chat and saw the message.
    /// Corresponds to whatsmeow "read".
    /// </summary>
    Read,

    /// <summary>
    /// Current user read a message from a different device (with read receipts disabled).
    /// Corresponds to whatsmeow "read-self".
    /// </summary>
    ReadSelf,

    /// <summary>
    /// User opened a view-once media message.
    /// Corresponds to whatsmeow "played".
    /// </summary>
    Played,

    /// <summary>
    /// Current user opened a view-once media message from a different device.
    /// Corresponds to whatsmeow "played-self".
    /// </summary>
    PlayedSelf,

    /// <summary>
    /// Server error occurred.
    /// Corresponds to whatsmeow "server-error".
    /// </summary>
    ServerError,

    /// <summary>
    /// Inactive receipt.
    /// Corresponds to whatsmeow "inactive".
    /// </summary>
    Inactive,

    /// <summary>
    /// Peer message receipt.
    /// Corresponds to whatsmeow "peer_msg".
    /// </summary>
    PeerMsg,

    /// <summary>
    /// History sync receipt.
    /// Corresponds to whatsmeow "hist_sync".
    /// </summary>
    HistorySync
}
