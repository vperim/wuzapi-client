using AwesomeAssertions;
using WuzApiClient.IntegrationTests.TestInfrastructure.Configuration;
using WuzApiClient.IntegrationTests.TestInfrastructure.Fixtures;
using WuzApiClient.Models.Common;

namespace WuzApiClient.IntegrationTests.User;

[Collection(WuzApiIntegrationCollection.Name)]
[Trait("Category", "Integration")]
[Trait("Category", TestCategories.Safe)]
public sealed class UserIntegrationTests
{
    private readonly WuzApiIntegrationFixture fixture;

    public UserIntegrationTests(WuzApiIntegrationFixture fixture)
    {
        this.fixture = fixture;
        TestConfiguration.Configuration = fixture.Configuration;
    }

    [Fact]
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
    [Trait("Category", "LiveApi")]
    public async Task GetContacts_ReturnsContactList()
    {
        // Act
        var result = await this.fixture.Client.GetContactsAsync();

        // Assert
        result.IsSuccess.Should().BeTrue($"Expected success but got error: {(result.IsFailure ? result.Error.ToString() : "N/A")}");
    }
}
