using WuzApiClient.Core.Interfaces;
using WuzApiClient.IntegrationTests.TestInfrastructure.Configuration;
using WuzApiClient.Models.Common;
using WuzApiClient.Models.Requests.Group;

namespace WuzApiClient.IntegrationTests.TestInfrastructure.Helpers;

/// <summary>
/// Manages test group lifecycle - creates and discovers test groups for integration tests.
/// </summary>
public sealed class TestGroupManager
{
    private const string TestGroupPrefix = "WaClient-IntTest-";
    private static string? cachedGroupId;
    private static readonly SemaphoreSlim GroupLock = new(1, 1);

    private readonly IWaClient client;

    public TestGroupManager(IWaClient client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    /// <summary>
    /// Gets a test group ID. If TestGroupId is configured, uses that.
    /// Otherwise, finds or creates a test group.
    /// </summary>
    public async Task<string> GetOrCreateTestGroupAsync()
    {
        // If explicitly configured, use that
        var configuredGroupId = TestConfiguration.TestGroupId;
        if (!string.IsNullOrWhiteSpace(configuredGroupId))
        {
            return configuredGroupId;
        }

        // Check if we already have a cached group ID
        if (cachedGroupId != null)
        {
            return cachedGroupId;
        }

        await GroupLock.WaitAsync();
        try
        {
            // Double-check after acquiring lock
            if (cachedGroupId != null)
            {
                return cachedGroupId;
            }

            // Try to find an existing test group
            var existingGroupId = await this.FindExistingTestGroupAsync();
            if (existingGroupId != null)
            {
                cachedGroupId = existingGroupId;
                return cachedGroupId;
            }

            // Create a new test group
            var newGroupId = await this.CreateTestGroupAsync();
            cachedGroupId = newGroupId;
            return cachedGroupId;
        }
        finally
        {
            GroupLock.Release();
        }
    }

    private async Task<string?> FindExistingTestGroupAsync()
    {
        var groupsResult = await this.client.GetGroupsAsync();
        if (groupsResult.IsFailure || groupsResult.Value?.Groups == null)
        {
            return null;
        }

        var testGroup = groupsResult.Value.Groups
            .FirstOrDefault(g => g.Name?.StartsWith(TestGroupPrefix, StringComparison.Ordinal) == true);

        return testGroup?.Jid;
    }

    private async Task<string> CreateTestGroupAsync()
    {
        var groupName = $"{TestGroupPrefix}{DateTime.UtcNow:yyyyMMdd-HHmmss}";
        var phone = Phone.Create(TestConfiguration.TestPhoneNumber);

        var request = new CreateGroupRequest
        {
            Name = groupName,
            Participants = [phone]
        };

        var result = await this.client.CreateGroupAsync(request);

        if (result.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to create test group: {result.Error}");
        }

        return result.Value.Jid!;
    }
}
