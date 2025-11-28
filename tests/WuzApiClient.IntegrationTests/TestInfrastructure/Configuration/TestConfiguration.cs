using Microsoft.Extensions.Configuration;

namespace WuzApiClient.IntegrationTests.TestInfrastructure.Configuration;

/// <summary>
/// Helper class for accessing test configuration values.
/// </summary>
public static class TestConfiguration
{
    private static IConfiguration? configuration;

    /// <summary>
    /// Gets or sets the configuration instance.
    /// </summary>
    public static IConfiguration Configuration
    {
        get => configuration ?? throw new InvalidOperationException("Configuration has not been initialized.");
        set => configuration = value;
    }

    /// <summary>
    /// Gets the test phone number from configuration.
    /// </summary>
    public static string TestPhoneNumber => Configuration["TestData:TestPhoneNumber"]
        ?? throw new InvalidOperationException("TestData:TestPhoneNumber is not configured.");

    /// <summary>
    /// Gets the test group ID from configuration.
    /// </summary>
    public static string TestGroupId => Configuration["TestData:TestGroupId"]
        ?? throw new InvalidOperationException("TestData:TestGroupId is not configured.");
}
