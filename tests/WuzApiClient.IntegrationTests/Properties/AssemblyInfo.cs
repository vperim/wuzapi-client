using WuzApiClient.IntegrationTests.TestInfrastructure.Ordering;
using Xunit;

// Register the tiered test orderer globally for all test classes in this assembly.
// Tests decorated with [TestTier] will execute in tier order (0 → 1 → 2 → 3).
// Undecorated tests will execute last (tier 99).
[assembly: TestCaseOrderer(
    ordererTypeName: "WuzApiClient.IntegrationTests.TestInfrastructure.Ordering.TieredTestOrderer",
    ordererAssemblyName: "WuzApiClient.IntegrationTests")]

// Disable parallel test execution to ensure deterministic ordering.
// Integration tests must run sequentially to avoid conflicts (rate limits, session state, etc.).
[assembly: CollectionBehavior(DisableTestParallelization = true)]
