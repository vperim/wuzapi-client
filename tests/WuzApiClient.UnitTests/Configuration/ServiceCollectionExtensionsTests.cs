using AwesomeAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WuzApiClient.Configuration;
using WuzApiClient.Core.Interfaces;

namespace WuzApiClient.UnitTests.Configuration;

[Trait("Category", "Unit")]
public sealed class ServiceCollectionExtensionsTests
{
    private static IConfiguration CreateValidConfiguration() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["WuzApi:BaseUrl"] = "http://localhost:8080/",
                ["WuzApi:UserToken"] = "test-token"
            })
            .Build();

    private static IConfiguration CreateValidAdminConfiguration() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["WuzApiAdmin:BaseUrl"] = "http://localhost:8080/",
                ["WuzApiAdmin:AdminToken"] = "admin-token"
            })
            .Build();

    [Fact]
    public void AddWuzApiClient_WithConfiguration_RegistersClient()
    {
        // Arrange
        var configuration = CreateValidConfiguration();
        var services = new ServiceCollection();

        // Act
        services.AddWuzApiClient(configuration);
        using var provider = services.BuildServiceProvider();

        // Assert
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IWaClient));
        descriptor.Should().NotBeNull();
        descriptor!.Lifetime.Should().Be(ServiceLifetime.Transient);
    }

    [Fact]
    public void AddWuzApiClient_WithConfigurationSection_RegistersClient()
    {
        // Arrange
        var configuration = CreateValidConfiguration();
        var section = configuration.GetSection("WuzApi");
        var services = new ServiceCollection();

        // Act
        services.AddWuzApiClient(section);
        using var provider = services.BuildServiceProvider();

        // Assert
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IWaClient));
        descriptor.Should().NotBeNull();
    }

    [Fact]
    public void AddWuzApiClient_WithAction_RegistersClient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddWuzApiClient(options =>
        {
            options.BaseUrl = "http://localhost:8080/";
            options.UserToken = "test-token";
        });
        using var provider = services.BuildServiceProvider();

        // Assert
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IWaClient));
        descriptor.Should().NotBeNull();
    }

    [Theory]
    [InlineData("IConfiguration")]
    [InlineData("IConfigurationSection")]
    [InlineData("Action")]
    public void AddWuzApiClient_NullServices_ThrowsArgumentNullException(string overload)
    {
        // Arrange
        IServiceCollection services = null!;

        // Act
        Action act = overload switch
        {
            "IConfiguration" => () => services.AddWuzApiClient(CreateValidConfiguration()),
            "IConfigurationSection" => () => services.AddWuzApiClient(CreateValidConfiguration().GetSection("WuzApi")),
            "Action" => () => services.AddWuzApiClient(_ => { }),
            _ => throw new InvalidOperationException()
        };

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("services");
    }

    [Fact]
    public void AddWuzApiClient_NullConfiguration_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        IConfiguration configuration = null!;

        // Act
        var act = () => services.AddWuzApiClient(configuration);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("configuration");
    }

    [Fact]
    public void AddWuzApiClient_NullConfigurationSection_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        IConfigurationSection section = null!;

        // Act
        var act = () => services.AddWuzApiClient(section);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("configurationSection");
    }

    [Fact]
    public void AddWuzApiClient_NullAction_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        Action<WuzApiOptions> configure = null!;

        // Act
        var act = () => services.AddWuzApiClient(configure);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("configure");
    }

    [Fact]
    public void AddWuzApiAdminClient_WithConfiguration_RegistersClient()
    {
        // Arrange
        var configuration = CreateValidAdminConfiguration();
        var services = new ServiceCollection();

        // Act
        services.AddWuzApiAdminClient(configuration);
        using var provider = services.BuildServiceProvider();

        // Assert
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IWuzApiAdminClient));
        descriptor.Should().NotBeNull();
        descriptor!.Lifetime.Should().Be(ServiceLifetime.Transient);
    }

    [Fact]
    public void AddWuzApiAdminClient_WithAction_RegistersClient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddWuzApiAdminClient(options =>
        {
            options.BaseUrl = "http://localhost:8080/";
            options.AdminToken = "admin-token";
        });
        using var provider = services.BuildServiceProvider();

        // Assert
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IWuzApiAdminClient));
        descriptor.Should().NotBeNull();
    }

    [Theory]
    [InlineData("IConfiguration")]
    [InlineData("IConfigurationSection")]
    [InlineData("Action")]
    public void AddWuzApiAdminClient_NullServices_ThrowsArgumentNullException(string overload)
    {
        // Arrange
        IServiceCollection services = null!;

        // Act
        Action act = overload switch
        {
            "IConfiguration" => () => services.AddWuzApiAdminClient(CreateValidAdminConfiguration()),
            "IConfigurationSection" => () => services.AddWuzApiAdminClient(CreateValidAdminConfiguration().GetSection("WuzApiAdmin")),
            "Action" => () => services.AddWuzApiAdminClient(_ => { }),
            _ => throw new InvalidOperationException()
        };

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("services");
    }

    [Fact]
    public void ResolveClient_ValidConfiguration_ReturnsClient()
    {
        // Arrange
        var configuration = CreateValidConfiguration();
        var services = new ServiceCollection();
        services.AddWuzApiClient(configuration);
        using var provider = services.BuildServiceProvider();

        // Act
        var client = provider.GetService<IWaClient>();

        // Assert
        client.Should().NotBeNull();
    }

    [Fact]
    public void ResolveAdminClient_ValidConfiguration_ReturnsClient()
    {
        // Arrange
        var configuration = CreateValidAdminConfiguration();
        var services = new ServiceCollection();
        services.AddWuzApiAdminClient(configuration);
        using var provider = services.BuildServiceProvider();

        // Act
        var client = provider.GetService<IWuzApiAdminClient>();

        // Assert
        client.Should().NotBeNull();
    }

    [Fact]
    public void AddWuzApiClient_ReturnsServiceCollection_ForChaining()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddWuzApiClient(CreateValidConfiguration());

        // Assert
        result.Should().BeSameAs(services);
    }

    [Fact]
    public void AddWuzApiAdminClient_ReturnsServiceCollection_ForChaining()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddWuzApiAdminClient(CreateValidAdminConfiguration());

        // Assert
        result.Should().BeSameAs(services);
    }
}
