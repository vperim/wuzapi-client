using System.Diagnostics;
using WuzApiClient.IntegrationTests.TestInfrastructure.Ordering;
using Xunit.Abstractions;

namespace WuzApiClient.IntegrationTests.TestInfrastructure.RateLimiting;

/// <summary>
/// Abstract base class for tests that require rate limiting to prevent API throttling.
/// Ensures sequential execution with configurable delays between operations.
/// </summary>
public abstract class ThrottledTestBase
{
    /// <summary>
    /// Global semaphore ensuring only one throttled operation executes at a time across all tests.
    /// </summary>
    private static readonly SemaphoreSlim GlobalThrottle = new(1, 1);

    /// <summary>
    /// Tracks the last execution time for enforcing minimum delays.
    /// </summary>
    private static DateTime lastExecutionTime = DateTime.MinValue;

    /// <summary>
    /// Lock object for thread-safe access to LastExecutionTime.
    /// </summary>
    private static readonly object TimeLock = new();

    /// <summary>
    /// Random number generator for jitter calculation (thread-safe).
    /// </summary>
    private static readonly Random Jitter = new();

    /// <summary>
    /// Gets the test output helper for diagnostic logging.
    /// </summary>
    protected ITestOutputHelper Output { get; }

    /// <summary>
    /// Gets the tier number for this test (used to determine rate limits).
    /// Derived classes should override if they know their tier statically.
    /// </summary>
    protected virtual int TestTier => TestTiers.Default;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThrottledTestBase"/> class.
    /// </summary>
    /// <param name="output">xUnit test output helper for diagnostic logging.</param>
    protected ThrottledTestBase(ITestOutputHelper output)
    {
        this.Output = output ?? throw new ArgumentNullException(nameof(output));
    }

    /// <summary>
    /// Executes an action with rate limiting based on the test tier.
    /// Ensures sequential execution and applies jitter-based delays.
    /// </summary>
    /// <param name="action">The action to execute with throttling.</param>
    /// <param name="tier">Optional tier override (uses TestTier property if not specified).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected async Task ThrottledExecuteAsync(Func<Task> action, int? tier = null)
    {
        var effectiveTier = tier ?? this.TestTier;
        var config = TierRateLimits.GetConfigForTier(effectiveTier);

        await GlobalThrottle.WaitAsync();
        try
        {
            // Calculate required delay based on tier configuration
            if (config.IsEnabled)
            {
                await this.ApplyRateLimitAsync(config, effectiveTier);
            }

            // Execute the actual operation
            var stopwatch = Stopwatch.StartNew();
            await action();
            stopwatch.Stop();

            this.Output.WriteLine($"[Throttle] Operation completed in {stopwatch.ElapsedMilliseconds}ms");

            // Update last execution time
            lock (TimeLock)
            {
                lastExecutionTime = DateTime.UtcNow;
            }
        }
        finally
        {
            GlobalThrottle.Release();
        }
    }

    /// <summary>
    /// Executes a function with rate limiting based on the test tier.
    /// Ensures sequential execution and applies jitter-based delays.
    /// </summary>
    /// <typeparam name="T">The return type of the function.</typeparam>
    /// <param name="function">The function to execute with throttling.</param>
    /// <param name="tier">Optional tier override (uses TestTier property if not specified).</param>
    /// <returns>A task representing the asynchronous operation with the function result.</returns>
    protected async Task<T> ThrottledExecuteAsync<T>(Func<Task<T>> function, int? tier = null)
    {
        var effectiveTier = tier ?? this.TestTier;
        var config = TierRateLimits.GetConfigForTier(effectiveTier);

        await GlobalThrottle.WaitAsync();
        try
        {
            // Calculate required delay based on tier configuration
            if (config.IsEnabled)
            {
                await this.ApplyRateLimitAsync(config, effectiveTier);
            }

            // Execute the actual operation
            var stopwatch = Stopwatch.StartNew();
            var result = await function();
            stopwatch.Stop();

            this.Output.WriteLine($"[Throttle] Operation completed in {stopwatch.ElapsedMilliseconds}ms");

            // Update last execution time
            lock (TimeLock)
            {
                lastExecutionTime = DateTime.UtcNow;
            }

            return result;
        }
        finally
        {
            GlobalThrottle.Release();
        }
    }

    /// <summary>
    /// Applies rate limiting delay with jitter.
    /// </summary>
    /// <param name="config">Rate limit configuration.</param>
    /// <param name="tier">The tier number (for logging).</param>
    /// <returns>A task representing the delay operation.</returns>
    private async Task ApplyRateLimitAsync(TierRateLimits.RateLimitConfig config, int tier)
    {
        DateTime lastExecution;
        lock (TimeLock)
        {
            lastExecution = lastExecutionTime;
        }

        // Calculate time since last execution
        var timeSinceLastExecution = DateTime.UtcNow - lastExecution;

        // Calculate delay with jitter (random value between min and max)
        int delayMs;
        lock (Jitter)
        {
            delayMs = Jitter.Next(config.MinDelayMs, config.MaxDelayMs + 1);
        }

        // Adjust delay based on time already elapsed
        var remainingDelayMs = Math.Max(0, delayMs - (int)timeSinceLastExecution.TotalMilliseconds);

        if (remainingDelayMs > 0)
        {
            var tierName = TestTiers.GetTierName(tier);
            this.Output.WriteLine(
                $"[Throttle] Tier {tier} ({tierName}): Applying {remainingDelayMs}ms delay " +
                $"(jitter: {config.MinDelayMs}-{config.MaxDelayMs}ms)");

            await Task.Delay(remainingDelayMs);
        }
    }
}
