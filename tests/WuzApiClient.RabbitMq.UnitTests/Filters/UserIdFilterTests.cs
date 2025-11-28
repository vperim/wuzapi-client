using AwesomeAssertions;
using Microsoft.Extensions.Options;
using Moq;
using WuzApiClient.RabbitMq.Configuration;
using WuzApiClient.RabbitMq.Filters;
using WuzApiClient.RabbitMq.UnitTests.TestInfrastructure.Builders;

namespace WuzApiClient.RabbitMq.UnitTests.Filters;

/// <summary>
/// Unit tests for <see cref="UserIdFilter"/>.
/// </summary>
[Trait("Category", "Unit")]
public sealed class UserIdFilterTests
{
    [Fact]
    public void Order_ShouldReturn101()
    {
        // Arrange
        var options = CreateOptions([]);
        var sut = new UserIdFilter(options);

        // Act
        var order = sut.Order;

        // Assert
        order.Should().Be(101);
    }

    [Fact]
    public void ShouldProcess_EmptyFilterUserIds_ReturnsTrue()
    {
        // Arrange
        var options = CreateOptions([]);
        var sut = new UserIdFilter(options);
        var evt = new TestWuzEventBuilder()
            .WithUserId("any-user-id")
            .Build();

        // Act
        var result = sut.ShouldProcess(evt);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("user-1")]
    [InlineData("user-2")]
    [InlineData("user-3")]
    public void ShouldProcess_MatchingUserId_ReturnsTrue(string userId)
    {
        // Arrange
        var options = CreateOptions(["user-1", "user-2", "user-3"]);
        var sut = new UserIdFilter(options);
        var evt = new TestWuzEventBuilder()
            .WithUserId(userId)
            .Build();

        // Act
        var result = sut.ShouldProcess(evt);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("user-4")]
    [InlineData("unknown-user")]
    [InlineData("")]
    public void ShouldProcess_NonMatchingUserId_ReturnsFalse(string userId)
    {
        // Arrange
        var options = CreateOptions(["user-1", "user-2"]);
        var sut = new UserIdFilter(options);
        var evt = new TestWuzEventBuilder()
            .WithUserId(userId)
            .Build();

        // Act
        var result = sut.ShouldProcess(evt);

        // Assert
        result.Should().BeFalse();
    }

    private static IOptions<WuzEventOptions> CreateOptions(HashSet<string> filterUserIds)
    {
        var options = new WuzEventOptions
        {
            FilterUserIds = filterUserIds,
        };

        var mock = new Mock<IOptions<WuzEventOptions>>();
        mock.Setup(o => o.Value).Returns(options);

        return mock.Object;
    }
}
