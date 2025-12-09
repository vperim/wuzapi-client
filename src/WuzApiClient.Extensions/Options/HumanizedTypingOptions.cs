using System;

namespace WuzApiClient.Extensions.Options;

/// <summary>
/// Options for humanized message sending that simulates natural typing behavior.
/// </summary>
public sealed record HumanizedTypingOptions
{
    /// <summary>
    /// Initial delay before typing starts (simulates reading/thinking time).
    /// </summary>
    public TimeSpan BaseDelay { get; init; } = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// Delay per character typed. Default ~50ms equals ~20 chars/sec or ~240 WPM.
    /// </summary>
    public TimeSpan PerCharacterDelay { get; init; } = TimeSpan.FromMilliseconds(50);

    /// <summary>
    /// Maximum total delay before sending (caps very long messages).
    /// </summary>
    public TimeSpan MaxDelay { get; init; } = TimeSpan.FromSeconds(8);

    /// <summary>
    /// Random jitter range applied to final delay for natural variation.
    /// The actual jitter will be a random value between -Jitter and +Jitter.
    /// </summary>
    public TimeSpan Jitter { get; init; } = TimeSpan.FromMilliseconds(300);

    /// <summary>
    /// Whether to send "composing" presence indicator before the delay.
    /// </summary>
    public bool ShowTypingIndicator { get; init; } = true;

    /// <summary>
    /// Default balanced options for natural typing simulation.
    /// BaseDelay: 500ms, PerChar: 50ms, MaxDelay: 8s, Jitter: 300ms.
    /// </summary>
    public static HumanizedTypingOptions Default { get; } = new();

    /// <summary>
    /// Fast typing simulation for quick responses.
    /// BaseDelay: 300ms, PerChar: 30ms, MaxDelay: 4s, Jitter: 150ms.
    /// </summary>
    public static HumanizedTypingOptions Fast { get; } = new()
    {
        BaseDelay = TimeSpan.FromMilliseconds(300),
        PerCharacterDelay = TimeSpan.FromMilliseconds(30),
        MaxDelay = TimeSpan.FromSeconds(4),
        Jitter = TimeSpan.FromMilliseconds(150)
    };

    /// <summary>
    /// Slow, thoughtful typing simulation.
    /// BaseDelay: 800ms, PerChar: 80ms, MaxDelay: 12s, Jitter: 500ms.
    /// </summary>
    public static HumanizedTypingOptions Slow { get; } = new()
    {
        BaseDelay = TimeSpan.FromMilliseconds(800),
        PerCharacterDelay = TimeSpan.FromMilliseconds(80),
        MaxDelay = TimeSpan.FromSeconds(12),
        Jitter = TimeSpan.FromMilliseconds(500)
    };
}
