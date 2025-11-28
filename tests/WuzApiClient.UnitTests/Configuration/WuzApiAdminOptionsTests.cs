using AwesomeAssertions;
using WuzApiClient.Configuration;
using WuzApiClient.Exceptions;

namespace WuzApiClient.UnitTests.Configuration;

/// <summary>
/// Unit tests for <see cref="WuzApiAdminOptions"/> validation and configuration.
/// </summary>
[Trait("Category", "Unit")]
public sealed class WuzApiAdminOptionsTests
{
    private readonly WuzApiAdminOptions sut = new();

    [Fact]
    public void Validate_ValidConfiguration_DoesNotThrow()
    {
        this.sut.BaseUrl = "https://api.example.com/";
        this.sut.AdminToken = "valid-admin-token";

        var act = () => this.sut.Validate();

        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(null, "BaseUrl is required.")]
    [InlineData("", "BaseUrl is required.")]
    [InlineData("   ", "BaseUrl is required.")]
    public void Validate_InvalidBaseUrl_ThrowsConfigurationException(string? baseUrl, string expectedMessage)
    {
        this.sut.BaseUrl = baseUrl!;
        this.sut.AdminToken = "valid-admin-token";

        var act = () => this.sut.Validate();

        act.Should().Throw<WuzApiConfigurationException>()
            .WithMessage(expectedMessage);
    }

    [Theory]
    [InlineData(null, "AdminToken is required.")]
    [InlineData("", "AdminToken is required.")]
    [InlineData("   ", "AdminToken is required.")]
    public void Validate_InvalidAdminToken_ThrowsConfigurationException(string? adminToken, string expectedMessage)
    {
        this.sut.BaseUrl = "https://api.example.com/";
        this.sut.AdminToken = adminToken!;

        var act = () => this.sut.Validate();

        act.Should().Throw<WuzApiConfigurationException>()
            .WithMessage(expectedMessage);
    }

    [Fact]
    public void Validate_InvalidBaseUrlFormat_ThrowsConfigurationException()
    {
        this.sut.BaseUrl = "not-a-valid-uri";
        this.sut.AdminToken = "valid-admin-token";

        var act = () => this.sut.Validate();

        act.Should().Throw<WuzApiConfigurationException>()
            .WithMessage("BaseUrl must be a valid absolute URI.");
    }

    [Fact]
    public void Validate_NonHttpScheme_ThrowsConfigurationException()
    {
        this.sut.BaseUrl = "ftp://files.example.com/";
        this.sut.AdminToken = "valid-admin-token";

        var act = () => this.sut.Validate();

        act.Should().Throw<WuzApiConfigurationException>()
            .WithMessage("BaseUrl must use http or https scheme.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_NonPositiveTimeout_ThrowsConfigurationException(int timeoutSeconds)
    {
        this.sut.BaseUrl = "https://api.example.com/";
        this.sut.AdminToken = "valid-admin-token";
        this.sut.TimeoutSeconds = timeoutSeconds;

        var act = () => this.sut.Validate();

        act.Should().Throw<WuzApiConfigurationException>()
            .WithMessage("TimeoutSeconds must be a positive value.");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(30)]
    [InlineData(120)]
    public void Timeout_ReturnsTimeSpanFromSeconds(int seconds)
    {
        this.sut.TimeoutSeconds = seconds;

        var result = this.sut.Timeout;

        result.Should().Be(TimeSpan.FromSeconds(seconds));
    }

    [Fact]
    public void DefaultValues_AreCorrect()
    {
        var options = new WuzApiAdminOptions();

        options.BaseUrl.Should().Be("http://localhost:8080/");
        options.TimeoutSeconds.Should().Be(30);
        options.AdminToken.Should().BeEmpty();
    }

    [Fact]
    public void SectionName_IsWuzApiAdmin()
    {
        WuzApiAdminOptions.SectionName.Should().Be("WuzApiAdmin");
    }
}
