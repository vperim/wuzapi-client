using AwesomeAssertions;
using WuzApiClient.RabbitMq.Configuration;

namespace WuzApiClient.RabbitMq.UnitTests.Configuration;

/// <summary>
/// Unit tests for <see cref="WuzEventOptions"/> configuration and validation.
/// </summary>
[Trait("Category", "Unit")]
public sealed class WuzEventOptionsTests
{
    private readonly WuzEventOptions sut = new();

    [Fact]
    public void DefaultValues_AreSetCorrectly()
    {
        // Arrange
        var options = new WuzEventOptions();

        // Assert
        options.ConnectionString.Should().BeEmpty();
        options.QueueName.Should().Be("whatsapp_events");
        options.ConsumerTagPrefix.Should().Be("wuzapi-consumer");
        options.PrefetchCount.Should().Be(10);
        options.AutoAck.Should().BeFalse();
        options.MaxReconnectAttempts.Should().Be(10);
        options.ReconnectDelay.Should().Be(TimeSpan.FromSeconds(3));
        options.MaxConcurrentMessages.Should().Be(Environment.ProcessorCount);
        options.SubscribedEventTypes.Should().BeEmpty();
        options.FilterUserIds.Should().BeEmpty();
        options.FilterInstanceNames.Should().BeEmpty();
    }

    [Fact]
    public void Validate_ValidConnectionString_DoesNotThrow()
    {
        // Arrange
        this.sut.ConnectionString = "amqp://user:pass@localhost:5672/vhost";

        // Act
        var act = () => this.sut.Validate();

        // Assert
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_EmptyConnectionString_ThrowsConfigurationException(string? connectionString)
    {
        // Arrange
        this.sut.ConnectionString = connectionString!;

        // Act
        var act = () => this.sut.Validate();

        // Assert
        act.Should().Throw<WuzEventsConfigurationException>()
            .WithMessage("*ConnectionString*required*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_EmptyQueueName_ThrowsConfigurationException(string? queueName)
    {
        // Arrange
        this.sut.ConnectionString = "amqp://localhost";
        this.sut.QueueName = queueName!;

        // Act
        var act = () => this.sut.Validate();

        // Assert
        act.Should().Throw<WuzEventsConfigurationException>()
            .WithMessage("*QueueName*required*");
    }

    [Fact]
    public void Validate_ZeroPrefetchCount_ThrowsConfigurationException()
    {
        // Arrange
        this.sut.ConnectionString = "amqp://localhost";
        this.sut.PrefetchCount = 0;

        // Act
        var act = () => this.sut.Validate();

        // Assert
        act.Should().Throw<WuzEventsConfigurationException>()
            .WithMessage("*PrefetchCount*greater than 0*");
    }

    [Fact]
    public void Validate_NegativeMaxReconnectAttempts_ThrowsConfigurationException()
    {
        // Arrange
        this.sut.ConnectionString = "amqp://localhost";
        this.sut.MaxReconnectAttempts = -1;

        // Act
        var act = () => this.sut.Validate();

        // Assert
        act.Should().Throw<WuzEventsConfigurationException>()
            .WithMessage("*MaxReconnectAttempts*cannot be negative*");
    }

    [Fact]
    public void Validate_NegativeReconnectDelay_ThrowsConfigurationException()
    {
        // Arrange
        this.sut.ConnectionString = "amqp://localhost";
        this.sut.ReconnectDelay = TimeSpan.FromSeconds(-1);

        // Act
        var act = () => this.sut.Validate();

        // Assert
        act.Should().Throw<WuzEventsConfigurationException>()
            .WithMessage("*ReconnectDelay*cannot be negative*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidMaxConcurrentMessages_ThrowsConfigurationException(int maxConcurrentMessages)
    {
        // Arrange
        this.sut.ConnectionString = "amqp://localhost";
        this.sut.MaxConcurrentMessages = maxConcurrentMessages;

        // Act
        var act = () => this.sut.Validate();

        // Assert
        act.Should().Throw<WuzEventsConfigurationException>()
            .WithMessage("*MaxConcurrentMessages*greater than 0*");
    }
}
