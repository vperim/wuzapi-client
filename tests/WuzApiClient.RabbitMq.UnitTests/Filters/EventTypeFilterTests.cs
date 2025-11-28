using AwesomeAssertions;
using Microsoft.Extensions.Options;
using Moq;
using WuzApiClient.RabbitMq.Configuration;
using WuzApiClient.RabbitMq.Filters;
using WuzApiClient.RabbitMq.UnitTests.TestInfrastructure.Builders;

namespace WuzApiClient.RabbitMq.UnitTests.Filters;

/// <summary>
/// Unit tests for <see cref="EventTypeFilter"/>.
/// </summary>
[Trait("Category", "Unit")]
public sealed class EventTypeFilterTests
{
    [Fact]
    public void Order_ShouldReturn100()
    {
        // Arrange
        var options = CreateOptions([]);
        var sut = new EventTypeFilter(options);

        // Act
        var order = sut.Order;

        // Assert
        order.Should().Be(100);
    }

    [Fact]
    public void ShouldProcess_EmptySubscribedEventTypes_ReturnsTrue()
    {
        // Arrange
        var options = CreateOptions([]);
        var sut = new EventTypeFilter(options);
        var evt = new TestWuzEventBuilder()
            .WithType("Message")
            .Build();

        // Act
        var result = sut.ShouldProcess(evt);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("Message")]
    [InlineData("Presence")]
    [InlineData("Receipt")]
    public void ShouldProcess_MatchingEventType_ReturnsTrue(string eventType)
    {
        // Arrange
        var options = CreateOptions(["Message", "Presence", "Receipt"]);
        var sut = new EventTypeFilter(options);
        var evt = new TestWuzEventBuilder()
            .WithType(eventType)
            .Build();

        // Act
        var result = sut.ShouldProcess(evt);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("CallOffer")]
    [InlineData("Unknown")]
    [InlineData("")]
    public void ShouldProcess_NonMatchingEventType_ReturnsFalse(string eventType)
    {
        // Arrange
        var options = CreateOptions(["Message", "Presence"]);
        var sut = new EventTypeFilter(options);
        var evt = new TestWuzEventBuilder()
            .WithType(eventType)
            .Build();

        // Act
        var result = sut.ShouldProcess(evt);

        // Assert
        result.Should().BeFalse();
    }

    private static IOptions<WuzEventOptions> CreateOptions(HashSet<string> subscribedEventTypes)
    {
        var options = new WuzEventOptions
        {
            SubscribedEventTypes = subscribedEventTypes,
        };

        var mock = new Mock<IOptions<WuzEventOptions>>();
        mock.Setup(o => o.Value).Returns(options);

        return mock.Object;
    }
}
