using AwesomeAssertions;
using WuzApiClient.Core.Implementations;

namespace WuzApiClient.UnitTests.Core.Implementations;

/// <summary>
/// Unit tests for WaClient constructor validation.
/// </summary>
[Trait("Category", "Unit")]
public sealed class WaClientConstructorTests
{
    [Fact]
    public void Constructor_NullHttpClient_ThrowsArgumentNullException()
    {
        var act = () => new WaClient(null!, "valid-token");

        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("httpClient");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_InvalidUserToken_ThrowsArgumentException(string? userToken)
    {
        using var httpClient = new HttpClient();

        var act = () => new WaClient(httpClient, userToken!);

        act.Should().Throw<ArgumentException>()
            .WithMessage("User token is required.*")
            .And.ParamName.Should().Be("userToken");
    }

    [Fact]
    public void Constructor_ValidParameters_CreatesInstance()
    {
        using var httpClient = new HttpClient();

        var client = new WaClient(httpClient, "valid-token");

        client.Should().NotBeNull();
    }
}
