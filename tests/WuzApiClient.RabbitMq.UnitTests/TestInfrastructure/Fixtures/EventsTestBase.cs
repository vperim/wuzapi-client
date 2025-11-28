using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace WuzApiClient.RabbitMq.UnitTests.TestInfrastructure.Fixtures;

/// <summary>
/// Base class for event tests providing common test infrastructure.
/// </summary>
public abstract class EventsTestBase
{
    /// <summary>
    /// Gets the service collection for test configuration.
    /// </summary>
    protected IServiceCollection Services { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventsTestBase"/> class.
    /// </summary>
    protected EventsTestBase()
    {
        this.Services = this.CreateServiceCollection();
    }

    /// <summary>
    /// Creates a new service collection for testing.
    /// </summary>
    /// <returns>A new service collection instance.</returns>
    protected virtual IServiceCollection CreateServiceCollection()
    {
        return new ServiceCollection();
    }

    /// <summary>
    /// Creates a mock logger for the specified type.
    /// </summary>
    /// <typeparam name="T">The type the logger is for.</typeparam>
    /// <returns>A mock logger instance.</returns>
    protected Mock<ILogger<T>> CreateMockLogger<T>()
    {
        return new Mock<ILogger<T>>();
    }

    /// <summary>
    /// Builds a service provider from the configured services.
    /// </summary>
    /// <returns>The built service provider.</returns>
    protected IServiceProvider BuildServiceProvider()
    {
        return this.Services.BuildServiceProvider();
    }

    /// <summary>
    /// Creates a service scope for testing.
    /// </summary>
    /// <returns>A new service scope.</returns>
    protected IServiceScope CreateServiceScope()
    {
        var provider = this.BuildServiceProvider();
        return provider.CreateScope();
    }
}
