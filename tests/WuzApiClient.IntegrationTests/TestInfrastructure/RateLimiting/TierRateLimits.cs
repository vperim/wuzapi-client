namespace WuzApiClient.IntegrationTests.TestInfrastructure.RateLimiting;

/// <summary>
/// Defines rate limiting configuration for each test tier.
/// Rate limits prevent WhatsApp API throttling and ensure test stability.
/// </summary>
public static class TierRateLimits
{
    /// <summary>
    /// Rate limit configuration for a specific tier.
    /// </summary>
    public sealed class RateLimitConfig
    {
        /// <summary>
        /// Gets the minimum delay in milliseconds before executing the next operation.
        /// </summary>
        public int MinDelayMs { get; init; }

        /// <summary>
        /// Gets the maximum delay in milliseconds before executing the next operation.
        /// Actual delay is randomized between min and max (jitter).
        /// </summary>
        public int MaxDelayMs { get; init; }

        /// <summary>
        /// Gets a value indicating whether this tier requires rate limiting.
        /// </summary>
        public bool IsEnabled => MinDelayMs > 0 || MaxDelayMs > 0;
    }

    /// <summary>
    /// Tier 0 (Read-Only): Minimal delays for safe, non-mutating operations.
    /// Examples: session status, get contacts, user info.
    /// </summary>
    public static RateLimitConfig ReadOnly { get; set; } = new()
    {
        MinDelayMs = 500,
        MaxDelayMs = 1000
    };

    /// <summary>
    /// Tier 1 (Messaging): Moderate delays to prevent WhatsApp rate limiting.
    /// Examples: send text, send image, mark read.
    /// WhatsApp enforces strict rate limits on message sending.
    /// </summary>
    public static RateLimitConfig Messaging { get; set; } = new()
    {
        MinDelayMs = 3000,
        MaxDelayMs = 5000
    };

    /// <summary>
    /// Tier 2 (State-Modifying): Moderate delays for configuration changes.
    /// Examples: set webhook, create group, update group info.
    /// </summary>
    public static RateLimitConfig StateModifying { get; set; } = new()
    {
        MinDelayMs = 2000,
        MaxDelayMs = 3000
    };

    /// <summary>
    /// Tier 3 (Destructive): Highest delays for critical operations.
    /// Examples: disconnect, logout, delete group.
    /// These operations should be infrequent and carefully spaced.
    /// </summary>
    public static RateLimitConfig Destructive { get; set; } = new()
    {
        MinDelayMs = 5000,
        MaxDelayMs = 7000
    };

    /// <summary>
    /// Default tier: No rate limiting for unclassified tests.
    /// </summary>
    public static RateLimitConfig Default { get; set; } = new()
    {
        MinDelayMs = 0,
        MaxDelayMs = 0
    };

    /// <summary>
    /// Gets the rate limit configuration for a given tier number.
    /// </summary>
    /// <param name="tier">The tier number (0-3, or 99 for default).</param>
    /// <returns>Rate limit configuration for the tier.</returns>
    public static RateLimitConfig GetConfigForTier(int tier)
    {
        return tier switch
        {
            0 => ReadOnly,
            1 => Messaging,
            2 => StateModifying,
            3 => Destructive,
            99 => Default,
            _ => Default
        };
    }
}
