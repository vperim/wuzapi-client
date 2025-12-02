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
                ["WuzApi:TimeoutSeconds"] = "30"
            })
            .Build();

    [Fact]
    public void AddWuzApi_WithConfiguration_RegistersFactories()
    {
        // Arrange
        var configuration = CreateValidConfiguration();
        var services = new ServiceCollection();

        // Act
        services.AddWuzApi(configuration);

        // Assert
        var clientFactoryDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IWaClientFactory));
        clientFactoryDescriptor.Should().NotBeNull();
        clientFactoryDescriptor!.Lifetime.Should().Be(ServiceLifetime.Singleton);

        var adminFactoryDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IWuzApiAdminClientFactory));
        adminFactoryDescriptor.Should().NotBeNull();
        adminFactoryDescriptor!.Lifetime.Should().Be(ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddWuzApi_WithConfigurationSection_RegistersFactories()
    {
        // Arrange
        var configuration = CreateValidConfiguration();
        var section = configuration.GetSection("WuzApi");
        var services = new ServiceCollection();

        // Act
        services.AddWuzApi(section);

        // Assert
        var clientFactoryDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IWaClientFactory));
        clientFactoryDescriptor.Should().NotBeNull();
    }

    [Fact]
    public void AddWuzApi_WithAction_RegistersFactories()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddWuzApi(options =>
        {
            options.BaseUrl = "http://localhost:8080/";
        });

        // Assert
        var clientFactoryDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IWaClientFactory));
        clientFactoryDescriptor.Should().NotBeNull();

        var adminFactoryDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IWuzApiAdminClientFactory));
        adminFactoryDescriptor.Should().NotBeNull();
    }

    [Fact]
    public void AddWuzApi_WithHttpClientBuilder_AllowsCustomization()
    {
        // Arrange
        var services = new ServiceCollection();
        var builderInvoked = false;

        // Act
        services.AddWuzApi(
            options => { options.BaseUrl = "http://localhost:8080/"; },
            builder =>
            {
                builderInvoked = true;
                builder.SetHandlerLifetime(TimeSpan.FromMinutes(5));
            });

        // Assert
        builderInvoked.Should().BeTrue();
    }

    [Theory]
    [InlineData("IConfiguration")]
    [InlineData("IConfigurationSection")]
    [InlineData("Action")]
    public void AddWuzApi_NullServices_ThrowsArgumentNullException(string overload)
    {
        // Arrange
        IServiceCollection services = null!;

        // Act
        Action act = overload switch
        {
            "IConfiguration" => () => services.AddWuzApi(CreateValidConfiguration()),
            "IConfigurationSection" => () => services.AddWuzApi(CreateValidConfiguration().GetSection("WuzApi")),
            "Action" => () => services.AddWuzApi(_ => { }),
            _ => throw new InvalidOperationException()
        };

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("services");
    }

    [Fact]
    public void AddWuzApi_NullConfiguration_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        IConfiguration configuration = null!;

        // Act
        var act = () => services.AddWuzApi(configuration);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("configuration");
    }

    [Fact]
    public void AddWuzApi_NullConfigurationSection_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        IConfigurationSection section = null!;

        // Act
        var act = () => services.AddWuzApi(section);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("configurationSection");
    }

    [Fact]
    public void AddWuzApi_NullAction_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        Action<WuzApiOptions> configure = null!;

        // Act
        var act = () => services.AddWuzApi(configure);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("configure");
    }

    [Fact]
    public void AddWuzApi_NullHttpClientBuilder_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        Action<IHttpClientBuilder> configureHttpClient = null!;

        // Act
        var act = () => services.AddWuzApi(_ => { }, configureHttpClient);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("configureHttpClient");
    }

    [Fact]
    public void ResolveFactory_ValidConfiguration_ReturnsFactory()
    {
        // Arrange
        var configuration = CreateValidConfiguration();
        var services = new ServiceCollection();
        services.AddWuzApi(configuration);
        using var provider = services.BuildServiceProvider();

        // Act
        var factory = provider.GetService<IWaClientFactory>();

        // Assert
        factory.Should().NotBeNull();
    }

    [Fact]
    public void ResolveAdminFactory_ValidConfiguration_ReturnsFactory()
    {
        // Arrange
        var configuration = CreateValidConfiguration();
        var services = new ServiceCollection();
        services.AddWuzApi(configuration);
        using var provider = services.BuildServiceProvider();

        // Act
        var factory = provider.GetService<IWuzApiAdminClientFactory>();

        // Assert
        factory.Should().NotBeNull();
    }

    [Fact]
    public void Factory_CreateClient_ReturnsClient()
    {
        // Arrange
        var configuration = CreateValidConfiguration();
        var services = new ServiceCollection();
        services.AddWuzApi(configuration);
        using var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<IWaClientFactory>();

        // Act
        var client = factory.CreateClient("test-token");

        // Assert
        client.Should().NotBeNull();
    }

    [Fact]
    public void AdminFactory_CreateClient_ReturnsClient()
    {
        // Arrange
        var configuration = CreateValidConfiguration();
        var services = new ServiceCollection();
        services.AddWuzApi(configuration);
        using var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<IWuzApiAdminClientFactory>();

        // Act
        var client = factory.CreateClient("admin-token");

        // Assert
        client.Should().NotBeNull();
    }

    [Fact]
    public void AddWuzApi_ReturnsServiceCollection_ForChaining()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddWuzApi(CreateValidConfiguration());

        // Assert
        result.Should().BeSameAs(services);
    }

    [Fact]
    public void AddWuzApi_CalledMultipleTimes_DoesNotDuplicateRegistrations()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddWuzApi(_ => { });
        services.AddWuzApi(_ => { });

        // Assert
        var factoryCount = services.Count(d => d.ServiceType == typeof(IWaClientFactory));
        factoryCount.Should().Be(1);
    }
}
