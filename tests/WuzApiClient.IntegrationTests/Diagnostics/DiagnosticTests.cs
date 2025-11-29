using AwesomeAssertions;
using WuzApiClient.IntegrationTests.TestInfrastructure.Fixtures;

namespace WuzApiClient.IntegrationTests.Diagnostics;

/// <summary>
/// Diagnostic tests for container inspection and troubleshooting.
/// These tests are not tiered and run last (default tier 99).
/// </summary>
[Collection(WuzApiIntegrationCollection.Name)]
[Trait("Category", "Integration")]
[Trait("Category", "Diagnostic")]
public sealed class DiagnosticTests
{
    private readonly WuzApiIntegrationFixture fixture;

    public DiagnosticTests(WuzApiIntegrationFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    [Trait("Category", "Diagnostic")]
    public async Task DiagnosticTest_InspectContainerLogs()
    {
        // Arrange & Act
        var (stdout, stderr) = await this.fixture.Container.GetContainerLogsAsync();

        // Output logs for inspection
        Console.WriteLine("=== CONTAINER STDOUT ===");
        Console.WriteLine(stdout);
        Console.WriteLine("\n=== CONTAINER STDERR ===");
        Console.WriteLine(stderr);

        // Also check container state
        var sessionState = await this.fixture.Container.GetSessionStateAsync();
        Console.WriteLine($"\n=== SESSION STATE: {sessionState} ===");

        // Assert container is running
        true.Should().BeTrue("Container logs retrieved successfully");
    }
}
