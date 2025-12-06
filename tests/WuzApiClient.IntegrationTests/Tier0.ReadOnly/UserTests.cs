using AwesomeAssertions;
using WuzApiClient.IntegrationTests.TestInfrastructure.Configuration;
using WuzApiClient.IntegrationTests.TestInfrastructure.Fixtures;
using WuzApiClient.IntegrationTests.TestInfrastructure.Ordering;
using WuzApiClient.Common.Models;

namespace WuzApiClient.IntegrationTests.Tier0.ReadOnly;

/// <summary>
/// Tier 0 (ReadOnly) tests for user information operations.
/// These tests retrieve user data without modifying any state.
/// </summary>
[Collection(WuzApiIntegrationCollection.Name)]
[Trait("Category", "Integration")]
[Trait("Tier", "0")]
public sealed class UserTests
{
    private readonly WuzApiIntegrationFixture fixture;

    public UserTests(WuzApiIntegrationFixture fixture)
    {
        this.fixture = fixture;
        TestConfiguration.Configuration = fixture.Configuration;
    }

    [Fact]
    [TestTier(TestTiers.ReadOnly, order: 10)]
    [Trait("Category", "LiveApi")]
    [Trait("Category", TestCategories.RequiresRealData)]
    public async Task GetUserInfo_ValidPhone_ReturnsUserInfo()
    {
        // Arrange - requires a real phone number that exists on WhatsApp
        var phone = Phone.Create(TestConfiguration.TestPhoneNumber);

        // Act
        var result = await this.fixture.Client.GetUserInfoAsync(phone);

        // Assert
        result.IsSuccess.Should().BeTrue($"Expected success but got error: {(result.IsFailure ? result.Error.ToString() : "N/A")}");
        result.Value.Should().NotBeNull();
    }

    [Fact]
    [TestTier(TestTiers.ReadOnly, order: 11)]
    [Trait("Category", "LiveApi")]
    public async Task CheckPhones_ValidPhones_ReturnsResults()
    {
        // Arrange
        var phones = new[] { Phone.Create(TestConfiguration.TestPhoneNumber) };

        // Act
        var result = await this.fixture.Client.CheckPhonesAsync(phones);

        // Assert
        result.IsSuccess.Should().BeTrue($"Expected success but got error: {(result.IsFailure ? result.Error.ToString() : "N/A")}");
        result.Value.Users.Should().NotBeNull();
    }

    [Fact]
    [TestTier(TestTiers.ReadOnly, order: 12)]
    [Trait("Category", "LiveApi")]
    public async Task GetContacts_ReturnsContactList()
    {
        // Act
        var result = await this.fixture.Client.GetContactsAsync();

        // Assert
        result.IsSuccess.Should().BeTrue($"Expected success but got error: {(result.IsFailure ? result.Error.ToString() : "N/A")}");
    }
}
