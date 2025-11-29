using AwesomeAssertions;
using WuzApiClient.IntegrationTests.TestInfrastructure.Ordering;

namespace WuzApiClient.IntegrationTests.Meta;

/// <summary>
/// Meta-tests that verify the tiered test ordering infrastructure is working correctly.
/// These tests validate that tests execute in the expected tier order.
/// </summary>
[Trait("Category", "Meta")]
public sealed class OrderingVerificationTests
{
    private static readonly List<string> ExecutionOrder = [];
    private static readonly Lock LockObject = new();

    [Fact]
    [TestTier(TestTiers.ReadOnly, order: 0)]
    public void Tier0_Test0_OrderParameter_ExecutesFirst()
    {
        RecordExecution(nameof(Tier0_Test0_OrderParameter_ExecutesFirst));

        // Order parameter should be respected within the same tier
        // This test has order=0, so it executes first in Tier 0
        ExecutionOrder.Count.Should().Be(1, "Tier 0 Test 0 should execute first");
        ExecutionOrder[0].Should().Be(nameof(Tier0_Test0_OrderParameter_ExecutesFirst));
    }

    [Fact]
    [TestTier(TestTiers.ReadOnly, order: 1)]
    public void Tier0_Test1_ExecutesSecond()
    {
        RecordExecution(nameof(Tier0_Test1_ExecutesSecond));

        // This should be the second test (order=1 in Tier 0)
        ExecutionOrder.Count.Should().Be(2, "Two tests should have executed by now");
        ExecutionOrder[0].Should().Be(nameof(Tier0_Test0_OrderParameter_ExecutesFirst));
        ExecutionOrder[1].Should().Be(nameof(Tier0_Test1_ExecutesSecond));
    }

    [Fact]
    [TestTier(TestTiers.ReadOnly, order: 2)]
    public void Tier0_Test2_ExecutesThird()
    {
        RecordExecution(nameof(Tier0_Test2_ExecutesThird));

        // This should be the third test (order=2 in Tier 0)
        ExecutionOrder.Count.Should().Be(3, "Three tests should have executed by now");
        ExecutionOrder[2].Should().Be(nameof(Tier0_Test2_ExecutesThird));
    }

    [Fact]
    [TestTier(TestTiers.Messaging, order: 1)]
    public void Tier1_Test1_ExecutesFourth()
    {
        RecordExecution(nameof(Tier1_Test1_ExecutesFourth));

        // This should be the fourth test (first in Tier 1)
        ExecutionOrder.Count.Should().Be(4, "Four tests should have executed by now");
        ExecutionOrder[3].Should().Be(nameof(Tier1_Test1_ExecutesFourth));
    }

    [Fact]
    [TestTier(TestTiers.StateModifying, order: 1)]
    public void Tier2_Test1_ExecutesFifth()
    {
        RecordExecution(nameof(Tier2_Test1_ExecutesFifth));

        // This should be the fifth test (first in Tier 2)
        ExecutionOrder.Count.Should().Be(5, "Five tests should have executed by now");
        ExecutionOrder[4].Should().Be(nameof(Tier2_Test1_ExecutesFifth));
    }

    [Fact]
    [TestTier(TestTiers.Destructive, order: 1)]
    public void Tier3_Test1_ExecutesSixth()
    {
        RecordExecution(nameof(Tier3_Test1_ExecutesSixth));

        // This should be the sixth test (first in Tier 3)
        ExecutionOrder.Count.Should().Be(6, "Six tests should have executed by now");
        ExecutionOrder[5].Should().Be(nameof(Tier3_Test1_ExecutesSixth));
    }

    [Fact]
    public void UndecoratedTest_ExecutesLast()
    {
        RecordExecution(nameof(UndecoratedTest_ExecutesLast));

        // Undecorated tests should execute after all tiered tests (tier 99)
        ExecutionOrder.Count.Should().Be(7, "Seven tests should have executed by now");
        ExecutionOrder[6].Should().Be(nameof(UndecoratedTest_ExecutesLast));
    }

    /// <summary>
    /// Records test execution order in a thread-safe manner.
    /// </summary>
    /// <param name="testName">The name of the executing test.</param>
    private static void RecordExecution(string testName)
    {
        lock (LockObject)
        {
            ExecutionOrder.Add(testName);
        }
    }
}
