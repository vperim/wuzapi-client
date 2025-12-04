using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event for message delivery and read receipts.
/// Maps to whatsmeow events.Receipt with wuzapi additions.
/// </summary>
public sealed record ReceiptEvent
{
    // === MessageSource fields (embedded in Go) ===

    /// <summary>
    /// Gets the chat JID.
    /// </summary>
    [JsonPropertyName("Chat")]
    public string? Chat { get; init; }

    /// <summary>
    /// Gets the sender JID.
    /// </summary>
    [JsonPropertyName("Sender")]
    public string? Sender { get; init; }

    /// <summary>
    /// Gets whether the receipt is from the current user.
    /// </summary>
    [JsonPropertyName("IsFromMe")]
    public bool IsFromMe { get; init; }

    /// <summary>
    /// Gets whether the chat is a group.
    /// </summary>
    [JsonPropertyName("IsGroup")]
    public bool IsGroup { get; init; }

    // === Receipt fields ===

    /// <summary>
    /// Gets the message IDs this receipt applies to.
    /// </summary>
    [JsonPropertyName("MessageIDs")]
    public IReadOnlyList<string>? MessageIDs { get; init; }

    /// <summary>
    /// Gets the receipt timestamp.
    /// </summary>
    [JsonPropertyName("Timestamp")]
    public DateTimeOffset? Timestamp { get; init; }

    /// <summary>
    /// Gets the receipt type from whatsmeow (e.g., "read", "played").
    /// </summary>
    [JsonPropertyName("Type")]
    public string? ReceiptType { get; init; }

    /// <summary>
    /// Gets the message sender JID for group receipts.
    /// </summary>
    [JsonPropertyName("MessageSender")]
    public string? MessageSender { get; init; }

    // === wuzapi-added fields ===

    /// <summary>
    /// Gets the receipt state (added by wuzapi: "Read", "ReadSelf", "Delivered").
    /// This field is at the root level in wuzapi JSON.
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; init; }
}
