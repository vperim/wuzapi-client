using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models;

/// <summary>
/// Message information within a MessageEvent.
/// Maps to whatsmeow types.MessageInfo which embeds types.MessageSource.
/// </summary>
public sealed record MessageInfo
{
    // === MessageSource fields (embedded in Go) ===

    /// <summary>
    /// Gets the chat JID where the message was sent.
    /// </summary>
    [JsonPropertyName("Chat")]
    public string? Chat { get; init; }

    /// <summary>
    /// Gets the sender JID.
    /// </summary>
    [JsonPropertyName("Sender")]
    public string? Sender { get; init; }

    /// <summary>
    /// Gets whether the message was sent by the current user.
    /// </summary>
    [JsonPropertyName("IsFromMe")]
    public bool IsFromMe { get; init; }

    /// <summary>
    /// Gets whether the chat is a group chat or broadcast list.
    /// </summary>
    [JsonPropertyName("IsGroup")]
    public bool IsGroup { get; init; }

    /// <summary>
    /// Gets the addressing mode of the message (phone number or LID).
    /// </summary>
    [JsonPropertyName("AddressingMode")]
    public string? AddressingMode { get; init; }

    /// <summary>
    /// Gets the alternative address of the sender (LID).
    /// </summary>
    [JsonPropertyName("SenderAlt")]
    public string? SenderAlt { get; init; }

    /// <summary>
    /// Gets the alternative address of the recipient for DMs.
    /// </summary>
    [JsonPropertyName("RecipientAlt")]
    public string? RecipientAlt { get; init; }

    /// <summary>
    /// Gets the broadcast list owner JID.
    /// </summary>
    [JsonPropertyName("BroadcastListOwner")]
    public string? BroadcastListOwner { get; init; }

    /// <summary>
    /// Gets the broadcast recipients.
    /// </summary>
    [JsonPropertyName("BroadcastRecipients")]
    public IReadOnlyList<BroadcastRecipient>? BroadcastRecipients { get; init; }

    // === MessageInfo fields ===

    /// <summary>
    /// Gets the message ID.
    /// </summary>
    [JsonPropertyName("ID")]
    public string? Id { get; init; }

    /// <summary>
    /// Gets the server-assigned message ID.
    /// </summary>
    [JsonPropertyName("ServerID")]
    public int ServerID { get; init; }

    /// <summary>
    /// Gets the message type (e.g., "text", "image").
    /// </summary>
    [JsonPropertyName("Type")]
    public string? Type { get; init; }

    /// <summary>
    /// Gets the push name (display name) of the sender.
    /// </summary>
    [JsonPropertyName("PushName")]
    public string? PushName { get; init; }

    /// <summary>
    /// Gets the message timestamp.
    /// </summary>
    [JsonPropertyName("Timestamp")]
    public DateTimeOffset? Timestamp { get; init; }

    /// <summary>
    /// Gets the message category.
    /// </summary>
    [JsonPropertyName("Category")]
    public string? Category { get; init; }

    /// <summary>
    /// Gets whether this is a multicast message.
    /// </summary>
    [JsonPropertyName("Multicast")]
    public bool Multicast { get; init; }

    /// <summary>
    /// Gets the media type if applicable.
    /// </summary>
    [JsonPropertyName("MediaType")]
    public string? MediaType { get; init; }

    /// <summary>
    /// Gets the edit attribute.
    /// </summary>
    [JsonPropertyName("Edit")]
    public string? Edit { get; init; }

    /// <summary>
    /// Gets the bot message info.
    /// </summary>
    [JsonPropertyName("MsgBotInfo")]
    public MsgBotInfo? MsgBotInfo { get; init; }

    /// <summary>
    /// Gets the message meta info.
    /// </summary>
    [JsonPropertyName("MsgMetaInfo")]
    public MsgMetaInfo? MsgMetaInfo { get; init; }

    /// <summary>
    /// Gets the verified name info.
    /// </summary>
    [JsonPropertyName("VerifiedName")]
    public VerifiedName? VerifiedName { get; init; }

    /// <summary>
    /// Gets metadata for direct messages sent from another device.
    /// </summary>
    [JsonPropertyName("DeviceSentMeta")]
    public DeviceSentMeta? DeviceSentMeta { get; init; }
}

/// <summary>
/// Broadcast recipient information.
/// </summary>
public sealed record BroadcastRecipient
{
    /// <summary>
    /// Gets the recipient JID.
    /// </summary>
    [JsonPropertyName("JID")]
    public string? Jid { get; init; }

    /// <summary>
    /// Gets the alternative JID (LID).
    /// </summary>
    [JsonPropertyName("LID")]
    public string? Lid { get; init; }
}

/// <summary>
/// Bot message information.
/// </summary>
public sealed record MsgBotInfo
{
    /// <summary>
    /// Gets the edit target ID.
    /// </summary>
    [JsonPropertyName("EditTargetID")]
    public string? EditTargetID { get; init; }

    /// <summary>
    /// Gets the edit type.
    /// </summary>
    [JsonPropertyName("EditType")]
    public string? EditType { get; init; }

    /// <summary>
    /// Gets the edit sender timestamp.
    /// </summary>
    [JsonPropertyName("EditSenderTimestampMS")]
    public DateTimeOffset? EditSenderTimestampMS { get; init; }
}

/// <summary>
/// Message meta information.
/// </summary>
public sealed record MsgMetaInfo
{
    /// <summary>
    /// Gets the target chat.
    /// </summary>
    [JsonPropertyName("TargetChat")]
    public string? TargetChat { get; init; }

    /// <summary>
    /// Gets the target ID.
    /// </summary>
    [JsonPropertyName("TargetID")]
    public string? TargetID { get; init; }

    /// <summary>
    /// Gets the target sender.
    /// </summary>
    [JsonPropertyName("TargetSender")]
    public string? TargetSender { get; init; }

    /// <summary>
    /// Gets the thread message ID.
    /// </summary>
    [JsonPropertyName("ThreadMessageID")]
    public string? ThreadMessageID { get; init; }

    /// <summary>
    /// Gets the thread message sender JID.
    /// </summary>
    [JsonPropertyName("ThreadMessageSenderJID")]
    public string? ThreadMessageSenderJID { get; init; }

    /// <summary>
    /// Gets the deprecated LID session.
    /// </summary>
    [JsonPropertyName("DeprecatedLIDSession")]
    public object? DeprecatedLIDSession { get; init; }
}

/// <summary>
/// Verified name information.
/// </summary>
public sealed record VerifiedName
{
    /// <summary>
    /// Gets the verified name.
    /// </summary>
    [JsonPropertyName("Name")]
    public string? Name { get; init; }
}

/// <summary>
/// Device sent metadata for messages from other devices.
/// </summary>
public sealed record DeviceSentMeta
{
    /// <summary>
    /// Gets the destination JID.
    /// </summary>
    [JsonPropertyName("DestinationJID")]
    public string? DestinationJID { get; init; }

    /// <summary>
    /// Gets the phash.
    /// </summary>
    [JsonPropertyName("Phash")]
    public string? Phash { get; init; }
}
