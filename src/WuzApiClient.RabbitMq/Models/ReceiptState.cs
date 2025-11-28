namespace WuzApiClient.RabbitMq.Models;

/// <summary>
/// Receipt state enumeration.
/// </summary>
public enum ReceiptState
{
    /// <summary>
    /// Message was delivered.
    /// </summary>
    Delivered,

    /// <summary>
    /// Message was read by recipient.
    /// </summary>
    Read,

    /// <summary>
    /// Message was read by self (on another device).
    /// </summary>
    ReadSelf,
}
