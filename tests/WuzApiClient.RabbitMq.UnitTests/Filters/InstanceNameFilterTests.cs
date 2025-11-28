using AwesomeAssertions;
using Microsoft.Extensions.Options;
using Moq;
using WuzApiClient.RabbitMq.Configuration;
using WuzApiClient.RabbitMq.Filters;
using WuzApiClient.RabbitMq.UnitTests.TestInfrastructure.Builders;

namespace WuzApiClient.RabbitMq.UnitTests.Filters;

/// <summary>
/// Unit tests for <see cref="InstanceNameFilter"/>.
/// </summary>
[Trait("Category", "Unit")]
public sealed class InstanceNameFilterTests
{
    [Fact]
    public void Order_ShouldReturn102()
    {
        // Arrange
        var options = CreateOptions([]);
        var sut = new InstanceNameFilter(options);

        // Act
        var order = sut.Order;

        // Assert
        order.Should().Be(102);
    }

    [Fact]
    public void ShouldProcess_EmptyFilterInstanceNames_ReturnsTrue()
    {
        // Arrange
        var options = CreateOptions([]);
        var sut = new InstanceNameFilter(options);
        var evt = new TestWuzEventBuilder()
            .WithInstanceName("any-instance")
            .Build();

        // Act
        var result = sut.ShouldProcess(evt);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("instance-1")]
    [InlineData("instance-2")]
    [InlineData("production")]
    public void ShouldProcess_MatchingInstanceName_ReturnsTrue(string instanceName)
    {
        // Arrange
        var options = CreateOptions(["instance-1", "instance-2", "production"]);
        var sut = new InstanceNameFilter(options);
        var evt = new TestWuzEventBuilder()
            .WithInstanceName(instanceName)
            .Build();

        // Act
        var result = sut.ShouldProcess(evt);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("instance-3")]
    [InlineData("staging")]
    [InlineData("")]
    public void ShouldProcess_NonMatchingInstanceName_ReturnsFalse(string instanceName)
    {
        // Arrange
        var options = CreateOptions(["instance-1", "production"]);
        var sut = new InstanceNameFilter(options);
        var evt = new TestWuzEventBuilder()
            .WithInstanceName(instanceName)
            .Build();

        // Act
        var result = sut.ShouldProcess(evt);

        // Assert
        result.Should().BeFalse();
    }

    private static IOptions<WuzEventOptions> CreateOptions(HashSet<string> filterInstanceNames)
    {
        var options = new WuzEventOptions
        {
            FilterInstanceNames = filterInstanceNames,
        };

        var mock = new Mock<IOptions<WuzEventOptions>>();
        mock.Setup(o => o.Value).Returns(options);

        return mock.Object;
    }
}
