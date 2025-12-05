using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Models.Events;

/// <summary>
/// Event when a newsletter is joined.
/// Maps to whatsmeow events.NewsletterJoin (embeds types.NewsletterMetadata).
/// </summary>
public sealed record NewsletterJoinEventEnvelope : WhatsAppEventEnvelope<NewsletterJoinEventData>
{
    [JsonPropertyName("event")]
    public override required NewsletterJoinEventData Event { get; init; }
}

/// <summary>
/// Event when a newsletter is joined.
/// Maps to whatsmeow events.NewsletterJoin (embeds types.NewsletterMetadata).
/// </summary>
public sealed record NewsletterJoinEventData
{
    /// <summary>
    /// Gets the newsletter JID.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    /// Gets the newsletter state information.
    /// </summary>
    [JsonPropertyName("state")]
    public NewsletterState? State { get; init; }

    /// <summary>
    /// Gets the newsletter thread metadata.
    /// </summary>
    [JsonPropertyName("thread_metadata")]
    public NewsletterThreadMetadata? ThreadMeta { get; init; }

    /// <summary>
    /// Gets the viewer-specific metadata.
    /// </summary>
    [JsonPropertyName("viewer_metadata")]
    public NewsletterViewerMetadata? ViewerMeta { get; init; }
}

/// <summary>
/// Newsletter state information.
/// </summary>
public sealed record NewsletterState
{
    /// <summary>
    /// Gets the newsletter state type.
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; init; }
}

/// <summary>
/// Newsletter thread metadata.
/// </summary>
public sealed record NewsletterThreadMetadata
{
    /// <summary>
    /// Gets the creation timestamp.
    /// </summary>
    [JsonPropertyName("creation_time")]
    public long? CreationTime { get; init; }

    /// <summary>
    /// Gets the invite code.
    /// </summary>
    [JsonPropertyName("invite")]
    public string? Invite { get; init; }

    /// <summary>
    /// Gets the newsletter name.
    /// </summary>
    [JsonPropertyName("name")]
    public NewsletterName? Name { get; init; }

    /// <summary>
    /// Gets the newsletter description.
    /// </summary>
    [JsonPropertyName("description")]
    public NewsletterDescription? Description { get; init; }

    /// <summary>
    /// Gets the subscriber count.
    /// </summary>
    [JsonPropertyName("subscribers_count")]
    public int? SubscribersCount { get; init; }

    /// <summary>
    /// Gets the verification state.
    /// </summary>
    [JsonPropertyName("verification")]
    public string? Verification { get; init; }

    /// <summary>
    /// Gets the picture information.
    /// </summary>
    [JsonPropertyName("picture")]
    public NewsletterPicture? Picture { get; init; }

    /// <summary>
    /// Gets the preview information.
    /// </summary>
    [JsonPropertyName("preview")]
    public NewsletterPreview? Preview { get; init; }

    /// <summary>
    /// Gets the reaction settings.
    /// </summary>
    [JsonPropertyName("settings")]
    public NewsletterSettings? Settings { get; init; }
}

/// <summary>
/// Newsletter name information.
/// </summary>
public sealed record NewsletterName
{
    /// <summary>
    /// Gets the newsletter name text.
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; init; }

    /// <summary>
    /// Gets the update timestamp.
    /// </summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; init; }
}

/// <summary>
/// Newsletter description information.
/// </summary>
public sealed record NewsletterDescription
{
    /// <summary>
    /// Gets the description text.
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; init; }

    /// <summary>
    /// Gets the update timestamp.
    /// </summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; init; }
}

/// <summary>
/// Newsletter picture information.
/// </summary>
public sealed record NewsletterPicture
{
    /// <summary>
    /// Gets the picture URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    /// <summary>
    /// Gets the direct path.
    /// </summary>
    [JsonPropertyName("direct_path")]
    public string? DirectPath { get; init; }
}

/// <summary>
/// Newsletter preview information.
/// </summary>
public sealed record NewsletterPreview
{
    /// <summary>
    /// Gets the preview URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; }
}

/// <summary>
/// Newsletter settings.
/// </summary>
public sealed record NewsletterSettings
{
    /// <summary>
    /// Gets the reaction codes setting.
    /// </summary>
    [JsonPropertyName("reaction_codes")]
    public NewsletterReactionSettings? ReactionCodes { get; init; }
}

/// <summary>
/// Newsletter reaction settings.
/// </summary>
public sealed record NewsletterReactionSettings
{
    /// <summary>
    /// Gets the value of the setting.
    /// </summary>
    [JsonPropertyName("value")]
    public string? Value { get; init; }
}

/// <summary>
/// Newsletter viewer metadata.
/// </summary>
public sealed record NewsletterViewerMetadata
{
    /// <summary>
    /// Gets the viewer's role ("subscriber", "guest", "admin", "owner").
    /// </summary>
    [JsonPropertyName("role")]
    public string? Role { get; init; }

    /// <summary>
    /// Gets the mute state ("on" or "off").
    /// </summary>
    [JsonPropertyName("mute")]
    public string? Mute { get; init; }
}
