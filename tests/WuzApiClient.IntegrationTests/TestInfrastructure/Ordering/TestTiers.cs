namespace WuzApiClient.IntegrationTests.TestInfrastructure.Ordering;

/// <summary>
/// Defines test tier constants for deterministic test ordering.
/// Tests are executed in tier order (0 → 1 → 2 → 3).
/// </summary>
public static class TestTiers
{
    /// <summary>
    /// Tier 0: Read-only operations that are safe to run anytime.
    /// These tests never modify session state or send messages.
    /// Examples: session status, get contacts, user info, group list.
    /// </summary>
    public const int ReadOnly = 0;

    /// <summary>
    /// Tier 1: Messaging operations with low session impact.
    /// These tests send messages but don't modify session configuration.
    /// Rate-limited to prevent WhatsApp throttling.
    /// Examples: send text, send image, mark read, chat presence.
    /// </summary>
    public const int Messaging = 1;

    /// <summary>
    /// Tier 2: State-modifying operations with medium impact.
    /// These tests change configuration or create/modify resources.
    /// Examples: set webhook, create group, update group info.
    /// </summary>
    public const int StateModifying = 2;

    /// <summary>
    /// Tier 3: Destructive operations with high impact.
    /// These tests disconnect sessions, delete resources, or perform irreversible actions.
    /// Always run last to avoid disrupting other tests.
    /// Examples: disconnect, logout, delete group.
    /// </summary>
    public const int Destructive = 3;

    /// <summary>
    /// Default tier for tests without explicit tier assignment.
    /// Undecorated tests execute after all tiered tests.
    /// </summary>
    public const int Default = 99;

    /// <summary>
    /// Gets a human-readable description for a tier number.
    /// </summary>
    /// <param name="tier">The tier number (0-3, or 99 for default).</param>
    /// <returns>Descriptive name for the tier.</returns>
    public static string GetTierName(int tier)
    {
        return tier switch
        {
            ReadOnly => "Read-Only",
            Messaging => "Messaging",
            StateModifying => "State-Modifying",
            Destructive => "Destructive",
            Default => "Default",
            _ => $"Unknown ({tier})"
        };
    }
}
