namespace WuzApiClient.Common.Enums;

/// <summary>
/// State of a message receipt as reported by wuzapi webhooks.
/// </summary>
/// <remarks>
/// Wuzapi only sends webhooks for Read, ReadSelf, and Delivered receipt states.
/// All other receipt types (Played, PlayedSelf, Retry, Inactive, etc.) are discarded
/// by wuzapi before reaching the webhook. See wmiau.go line 1414.
/// </remarks>
public enum ReceiptState
{
    /// <summary>
    /// Unknown or unrecognized state.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The recipient opened the chat and saw the message.
    /// Corresponds to whatsmeow ReceiptTypeRead.
    /// </summary>
    Read,

    /// <summary>
    /// The current user read a message from a different device (with read receipts disabled).
    /// Corresponds to whatsmeow ReceiptTypeReadSelf.
    /// </summary>
    ReadSelf,

    /// <summary>
    /// The message was delivered to the recipient's device.
    /// Corresponds to whatsmeow ReceiptTypeDelivered.
    /// </summary>
    Delivered
}
