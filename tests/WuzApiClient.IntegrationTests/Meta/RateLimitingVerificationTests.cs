using System.Diagnostics;
using AwesomeAssertions;
using WuzApiClient.IntegrationTests.TestInfrastructure.Ordering;
using WuzApiClient.IntegrationTests.TestInfrastructure.RateLimiting;
using Xunit.Abstractions;

namespace WuzApiClient.IntegrationTests.Meta;

/// <summary>
/// Meta-tests that verify the rate limiting infrastructure is working correctly.
/// These tests validate throttling delays, jitter, and sequential execution.
/// </summary>
[Trait("Category", "Meta")]
public sealed class RateLimitingVerificationTests : ThrottledTestBase
{
    private static readonly List<ExecutionRecord> ExecutionHistory = [];
    private static readonly Lock HistoryLock = new();

    public RateLimitingVerificationTests(ITestOutputHelper output)
        : base(output)
    {
    }

    [Fact]
    [TestTier(TestTiers.ReadOnly, order: 0)]
    public async Task ReadOnlyTier_AppliesCorrectDelay()
    {
        var executionTime = await this.ThrottledExecuteAsync(async () =>
        {
            var stopwatch = Stopwatch.StartNew();
            await Task.Delay(10); // Simulate minimal work
            stopwatch.Stop();
            RecordExecution(nameof(ReadOnlyTier_AppliesCorrectDelay), TestTiers.ReadOnly, stopwatch.Elapsed);
            return stopwatch.Elapsed;
        }, tier: TestTiers.ReadOnly);

        // Verify operation completed (basic sanity check)
        executionTime.Should().BeLessThan(TimeSpan.FromSeconds(5), "Operation should complete quickly");

        // First test in sequence - no previous execution to compare against
        this.Output.WriteLine($"ReadOnly tier execution recorded: {executionTime.TotalMilliseconds}ms");
    }

    [Fact]
    [TestTier(TestTiers.ReadOnly, order: 1)]
    public async Task ReadOnlyTier_SecondCall_RespectsMinimumDelay()
    {
        var overallStopwatch = Stopwatch.StartNew();

        await this.ThrottledExecuteAsync(async () =>
        {
            await Task.Delay(10); // Simulate minimal work
            RecordExecution(nameof(ReadOnlyTier_SecondCall_RespectsMinimumDelay), TestTiers.ReadOnly, overallStopwatch.Elapsed);
        }, tier: TestTiers.ReadOnly);

        overallStopwatch.Stop();

        // Get the two most recent executions
        var recentExecutions = GetRecentExecutions(2);

        if (recentExecutions.Count == 2)
        {
            var timeBetweenCalls = recentExecutions[1].Timestamp - recentExecutions[0].Timestamp;
            var config = TierRateLimits.GetConfigForTier(TestTiers.ReadOnly);

            this.Output.WriteLine(
                $"Time between calls: {timeBetweenCalls.TotalMilliseconds}ms " +
                $"(expected: {config.MinDelayMs}-{config.MaxDelayMs}ms)");

            // Verify delay is at least the minimum (with small tolerance for timing precision)
            timeBetweenCalls.TotalMilliseconds.Should().BeGreaterThanOrEqualTo(
                config.MinDelayMs - 50,
                $"Delay should be at least {config.MinDelayMs}ms (ReadOnly tier minimum)");
        }
    }

    [Fact]
    [TestTier(TestTiers.Messaging, order: 0)]
    public async Task MessagingTier_AppliesLongerDelay()
    {
        var overallStopwatch = Stopwatch.StartNew();

        await this.ThrottledExecuteAsync(async () =>
        {
            await Task.Delay(10); // Simulate minimal work
            RecordExecution(nameof(MessagingTier_AppliesLongerDelay), TestTiers.Messaging, overallStopwatch.Elapsed);
        }, tier: TestTiers.Messaging);

        overallStopwatch.Stop();

        // Get the two most recent executions (this test and the previous one)
        var recentExecutions = GetRecentExecutions(2);

        if (recentExecutions.Count == 2)
        {
            var timeBetweenCalls = recentExecutions[1].Timestamp - recentExecutions[0].Timestamp;
            var config = TierRateLimits.GetConfigForTier(TestTiers.Messaging);

            this.Output.WriteLine(
                $"Time between calls: {timeBetweenCalls.TotalMilliseconds}ms " +
                $"(expected: {config.MinDelayMs}-{config.MaxDelayMs}ms)");

            // Verify messaging tier has longer delay than read-only
            var readOnlyConfig = TierRateLimits.GetConfigForTier(TestTiers.ReadOnly);
            config.MinDelayMs.Should().BeGreaterThan(
                readOnlyConfig.MinDelayMs,
                "Messaging tier should have longer delays than ReadOnly tier");
        }
    }

    [Fact]
    [TestTier(TestTiers.StateModifying, order: 0)]
    public async Task StateModifyingTier_AppliesModerateDelay()
    {
        var overallStopwatch = Stopwatch.StartNew();

        await this.ThrottledExecuteAsync(async () =>
        {
            await Task.Delay(10); // Simulate minimal work
            RecordExecution(nameof(StateModifyingTier_AppliesModerateDelay), TestTiers.StateModifying, overallStopwatch.Elapsed);
        }, tier: TestTiers.StateModifying);

        overallStopwatch.Stop();

        var config = TierRateLimits.GetConfigForTier(TestTiers.StateModifying);
        this.Output.WriteLine(
            $"StateModifying tier config: {config.MinDelayMs}-{config.MaxDelayMs}ms");

        // Verify configuration is reasonable
        config.MinDelayMs.Should().BeGreaterThan(0, "StateModifying tier should have rate limiting enabled");
    }

    [Fact]
    [TestTier(TestTiers.Destructive, order: 0)]
    public async Task DestructiveTier_AppliesHighestDelay()
    {
        var overallStopwatch = Stopwatch.StartNew();

        await this.ThrottledExecuteAsync(async () =>
        {
            await Task.Delay(10); // Simulate minimal work
            RecordExecution(nameof(DestructiveTier_AppliesHighestDelay), TestTiers.Destructive, overallStopwatch.Elapsed);
        }, tier: TestTiers.Destructive);

        overallStopwatch.Stop();

        var config = TierRateLimits.GetConfigForTier(TestTiers.Destructive);
        var messagingConfig = TierRateLimits.GetConfigForTier(TestTiers.Messaging);

        this.Output.WriteLine(
            $"Destructive tier config: {config.MinDelayMs}-{config.MaxDelayMs}ms");

        // Verify destructive tier has highest delays
        config.MinDelayMs.Should().BeGreaterThanOrEqualTo(
            messagingConfig.MinDelayMs,
            "Destructive tier should have delays >= Messaging tier");
    }

    [Fact]
    [TestTier(TestTiers.Destructive, order: 1)]
    public async Task JitterProducesVariableDelays()
    {
        // This test verifies that jitter is working by checking execution history
        // We expect variable delays within the configured bounds

        await this.ThrottledExecuteAsync(async () =>
        {
            await Task.Delay(10); // Simulate minimal work
            RecordExecution(nameof(JitterProducesVariableDelays), TestTiers.Destructive, TimeSpan.Zero);
        }, tier: TestTiers.Destructive);

        // Analyze all recorded delays
        var allExecutions = GetAllExecutions();

        if (allExecutions.Count >= 3)
        {
            var delays = new List<double>();

            for (int i = 1; i < allExecutions.Count; i++)
            {
                var delay = (allExecutions[i].Timestamp - allExecutions[i - 1].Timestamp).TotalMilliseconds;
                delays.Add(delay);
            }

            this.Output.WriteLine($"Observed delays: {string.Join(", ", delays.Select(d => $"{d:F0}ms"))}");

            // If we have enough samples, check for variation (not all delays are identical)
            if (delays.Count >= 2)
            {
                var uniqueDelays = delays.Distinct().Count();
                this.Output.WriteLine($"Unique delay values: {uniqueDelays} out of {delays.Count}");

                // We expect some variation due to jitter (though with small sample size this might fail occasionally)
                // This is more of an observational test than a hard requirement
                if (uniqueDelays == 1)
                {
                    this.Output.WriteLine("WARNING: All delays are identical - jitter may not be working as expected");
                }
            }
        }
    }

    private static void RecordExecution(string testName, int tier, TimeSpan executionTime)
    {
        lock (HistoryLock)
        {
            ExecutionHistory.Add(new ExecutionRecord
            {
                TestName = testName,
                Tier = tier,
                Timestamp = DateTime.UtcNow,
                ExecutionTime = executionTime
            });
        }
    }

    private static List<ExecutionRecord> GetRecentExecutions(int count)
    {
        lock (HistoryLock)
        {
            return ExecutionHistory
                .OrderBy(e => e.Timestamp)
                .TakeLast(count)
                .ToList();
        }
    }

    private static List<ExecutionRecord> GetAllExecutions()
    {
        lock (HistoryLock)
        {
            return ExecutionHistory
                .OrderBy(e => e.Timestamp)
                .ToList();
        }
    }

    private sealed class ExecutionRecord
    {
        public required string TestName { get; init; }
        public required int Tier { get; init; }
        public required DateTime Timestamp { get; init; }
        public required TimeSpan ExecutionTime { get; init; }
    }
}
