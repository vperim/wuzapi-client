using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using WuzApiClient.Common.Models;
using WuzApiClient.Common.Serialization;

namespace WuzApiClient.RabbitMq.Models.Common;

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
    [JsonConverter(typeof(JidConverter))]
    public Jid? Chat { get; init; }

    /// <summary>
    /// Gets the sender JID.
    /// </summary>
    [JsonPropertyName("Sender")]
    [JsonConverter(typeof(JidConverter))]
    public Jid? Sender { get; init; }

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
    [JsonConverter(typeof(JidConverter))]
    public Jid? SenderAlt { get; init; }

    /// <summary>
    /// Gets the alternative address of the recipient for DMs.
    /// </summary>
    [JsonPropertyName("RecipientAlt")]
    [JsonConverter(typeof(JidConverter))]
    public Jid? RecipientAlt { get; init; }

    /// <summary>
    /// Gets the broadcast list owner JID.
    /// </summary>
    [JsonPropertyName("BroadcastListOwner")]
    [JsonConverter(typeof(JidConverter))]
    public Jid? BroadcastListOwner { get; init; }

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
    public int ServerId { get; init; }

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
    [JsonConverter(typeof(JidConverter))]
    public Jid? Jid { get; init; }

    /// <summary>
    /// Gets the alternative JID (LID).
    /// </summary>
    [JsonPropertyName("LID")]
    [JsonConverter(typeof(JidConverter))]
    public Jid? Lid { get; init; }
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
    public string? EditTargetId { get; init; }

    /// <summary>
    /// Gets the edit type.
    /// </summary>
    [JsonPropertyName("EditType")]
    public string? EditType { get; init; }

    /// <summary>
    /// Gets the edit sender timestamp.
    /// </summary>
    [JsonPropertyName("EditSenderTimestampMS")]
    public DateTimeOffset? EditSenderTimestampMs { get; init; }
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
    [JsonConverter(typeof(JidConverter))]
    public Jid? TargetChat { get; init; }

    /// <summary>
    /// Gets the target ID.
    /// </summary>
    [JsonPropertyName("TargetID")]
    public string? TargetId { get; init; }

    /// <summary>
    /// Gets the target sender.
    /// </summary>
    [JsonPropertyName("TargetSender")]
    [JsonConverter(typeof(JidConverter))]
    public Jid? TargetSender { get; init; }

    /// <summary>
    /// Gets the thread message ID.
    /// </summary>
    [JsonPropertyName("ThreadMessageID")]
    public string? ThreadMessageId { get; init; }

    /// <summary>
    /// Gets the thread message sender JID.
    /// </summary>
    [JsonPropertyName("ThreadMessageSenderJID")]
    [JsonConverter(typeof(JidConverter))]
    public Jid? ThreadMessageSenderJid { get; init; }

    /// <summary>
    /// Gets the deprecated LID session.
    /// </summary>
    [JsonPropertyName("DeprecatedLIDSession")]
    public object? DeprecatedLidSession { get; init; }
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
    [JsonConverter(typeof(JidConverter))]
    public Jid? DestinationJid { get; init; }

    /// <summary>
    /// Gets the phash.
    /// </summary>
    [JsonPropertyName("Phash")]
    public string? Phash { get; init; }
}
