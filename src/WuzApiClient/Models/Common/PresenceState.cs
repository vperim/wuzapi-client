namespace WuzApiClient.Models.Common;

/// <summary>
/// Defines presence states for chat indicators.
/// </summary>
public enum PresenceState
{
    /// <summary>
    /// User is available (default state).
    /// </summary>
    Available,

    /// <summary>
    /// User is typing a message.
    /// </summary>
    Composing,

    /// <summary>
    /// User is recording audio.
    /// </summary>
    Recording,

    /// <summary>
    /// User has paused typing/recording.
    /// </summary>
    Paused
}
