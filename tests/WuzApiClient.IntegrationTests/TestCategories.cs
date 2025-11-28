namespace WuzApiClient.IntegrationTests;

/// <summary>
/// Test category constants for organizing integration tests by safety level.
/// </summary>
public static class TestCategories
{
    /// <summary>
    /// Safe tests that only read data and don't modify any state.
    /// These can be run at any time without risk.
    /// Examples: GetGroups, GetContacts, GetSessionStatus, CheckPhones
    /// </summary>
    public const string Safe = "Safe";

    /// <summary>
    /// Tests that modify temporary/reversible state.
    /// Examples: CreateUser/DeleteUser, SetWebhook
    /// </summary>
    public const string MildlyDestructive = "MildlyDestructive";

    /// <summary>
    /// Tests that could break the session or send real messages.
    /// Examples: DisconnectSession, LogoutSession, SendTextMessage
    /// </summary>
    public const string Destructive = "Destructive";

    /// <summary>
    /// Tests that require an active WhatsApp session.
    /// </summary>
    public const string RequiresSession = "RequiresSession";

    /// <summary>
    /// Tests that require real phone numbers or group IDs.
    /// </summary>
    public const string RequiresRealData = "RequiresRealData";
}
