using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using WuzApiClient.RabbitMq.UnitTests.TestInfrastructure.Builders;
using WuzApiClient.RabbitMq.UnitTests.TestInfrastructure.Fixtures;
using WuzApiClient.RabbitMq.UnitTests.TestInfrastructure.Mocks;

namespace WuzApiClient.RabbitMq.UnitTests.TestInfrastructure;

/// <summary>
/// Tests to verify the test infrastructure works correctly.
/// </summary>
public sealed class InfrastructureTests : EventsTestBase
{
    [Fact]
    public void EventsTestBase_CreateServiceCollection_ShouldReturnServiceCollection()
    {
        // Act
        var services = this.CreateServiceCollection();

        // Assert
        services.Should().NotBeNull();
        services.Should().BeAssignableTo<IServiceCollection>();
    }

    [Fact]
    public void EventsTestBase_CreateMockLogger_ShouldReturnMockLogger()
    {
        // Act
        var logger = this.CreateMockLogger<InfrastructureTests>();

        // Assert
        logger.Should().NotBeNull();
        logger.Object.Should().NotBeNull();
    }

    [Fact]
    public void TestWuzEventBuilder_Build_ShouldCreateEventWithDefaults()
    {
        // Arrange
        var builder = new TestWuzEventBuilder();

        // Act
        var evt = builder.Build();

        // Assert
        evt.Should().NotBeNull();
        evt.EventType.Should().Be("TestEvent");
        evt.UserId.Should().Be("test-user");
        evt.InstanceName.Should().Be("test-instance");
    }

    [Fact]
    public void TestWuzEventBuilder_WithCustomValues_ShouldCreateEventWithCustomValues()
    {
        // Arrange & Act
        var evt = new TestWuzEventBuilder()
            .WithType("CustomType")
            .WithUserId("custom-user")
            .WithInstanceName("custom-instance")
            .Build();

        // Assert
        evt.EventType.Should().Be("CustomType");
        evt.UserId.Should().Be("custom-user");
        evt.InstanceName.Should().Be("custom-instance");
    }

    [Fact]
    public void MockRabbitMqConnection_DefaultState_ShouldBeConnected()
    {
        // Arrange
        var mock = new MockRabbitMqConnection();

        // Act & Assert
        mock.IsConnected.Should().BeTrue();
        mock.CreateChannelCallCount.Should().Be(0);
    }

    [Fact]
    public async Task MockRabbitMqConnection_CreateChannelAsync_ShouldTrackCalls()
    {
        // Arrange
        var mock = new MockRabbitMqConnection();

        // Act
        var channel = await mock.CreateChannelAsync();

        // Assert
        channel.Should().NotBeNull();
        mock.CreateChannelCallCount.Should().Be(1);
        mock.MethodCalls.Should().Contain("CreateChannelAsync");
    }

    [Fact]
    public async Task MockRabbitMqConnection_TryReconnectAsync_ShouldSucceedByDefault()
    {
        // Arrange
        var mock = new MockRabbitMqConnection { IsConnected = false };

        // Act
        var result = await mock.TryReconnectAsync();

        // Assert
        result.Should().BeTrue();
        mock.IsConnected.Should().BeTrue();
        mock.TryReconnectCallCount.Should().Be(1);
    }

    [Fact]
    public void MockServiceScopeFactory_CreateScope_ShouldTrackScopes()
    {
        // Arrange
        var factory = new MockServiceScopeFactory();

        // Act
        var scope = factory.CreateScope();

        // Assert
        scope.Should().NotBeNull();
        factory.CreateScopeCallCount.Should().Be(1);
        factory.CreatedScopes.Should().HaveCount(1);
    }

    [Fact]
    public void MockServiceProvider_AddService_ShouldReturnService()
    {
        // Arrange
        var provider = new MockServiceProvider();
        var service = "test-service";

        // Act
        provider.AddService<string>(service);
        var result = provider.GetService(typeof(string));

        // Assert
        result.Should().Be(service);
        provider.GetServiceCallCount.Should().Be(1);
    }

    [Fact]
    public void MockServiceScope_Dispose_ShouldSetIsDisposed()
    {
        // Arrange
        var provider = new MockServiceProvider();
        var scope = new MockServiceScope(provider);

        // Act
        scope.Dispose();

        // Assert
        scope.IsDisposed.Should().BeTrue();
    }
}
