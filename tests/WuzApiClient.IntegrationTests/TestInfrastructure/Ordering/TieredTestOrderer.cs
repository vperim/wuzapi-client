using Xunit.Abstractions;
using Xunit.Sdk;

namespace WuzApiClient.IntegrationTests.TestInfrastructure.Ordering;

/// <summary>
/// xUnit test case orderer that orders tests by tier, then by order within tier.
/// Tests decorated with <see cref="TestTierAttribute"/> execute in deterministic order.
/// </summary>
public sealed class TieredTestOrderer : ITestCaseOrderer
{
    /// <summary>
    /// Orders test cases by tier (ascending), then by order within tier (ascending),
    /// then alphabetically by display name for deterministic execution.
    /// </summary>
    /// <typeparam name="TTestCase">The type of test case.</typeparam>
    /// <param name="testCases">The collection of test cases to order.</param>
    /// <returns>Ordered collection of test cases.</returns>
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
        where TTestCase : ITestCase
    {
        var orderedTests = testCases
            .Select(testCase => new
            {
                TestCase = testCase,
                Metadata = ExtractTierMetadata(testCase)
            })
            .OrderBy(t => t.Metadata.Tier)
            .ThenBy(t => t.Metadata.Order)
            .ThenBy(t => t.TestCase.TestMethod.Method.Name)
            .Select(t => t.TestCase);

        return orderedTests;
    }

    /// <summary>
    /// Extracts tier and order metadata from a test case.
    /// </summary>
    /// <param name="testCase">The test case to examine.</param>
    /// <returns>Tier and order metadata, or default values if attribute not present.</returns>
    private static (int Tier, int Order) ExtractTierMetadata(ITestCase testCase)
    {
        var testMethod = testCase.TestMethod.Method;

        // Look for TestTierAttribute on the test method
        var tierAttribute = testMethod
            .GetCustomAttributes(typeof(TestTierAttribute))
            .FirstOrDefault();

        if (tierAttribute == null)
        {
            // No attribute found - use default tier (99) so undecorated tests run last
            return (TestTiers.Default, 0);
        }

        // Extract tier and order from attribute arguments
        var tier = tierAttribute.GetNamedArgument<int>(nameof(TestTierAttribute.Tier));
        var order = tierAttribute.GetNamedArgument<int>(nameof(TestTierAttribute.Order));

        return (tier, order);
    }
}
