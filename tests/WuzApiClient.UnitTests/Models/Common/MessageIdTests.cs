using AwesomeAssertions;
using WuzApiClient.Common.Models;
using WuzApiClient.Models.Common;

namespace WuzApiClient.UnitTests.Models.Common;

/// <summary>
/// Unit tests for <see cref="MessageId"/> domain type.
/// </summary>
[Trait("Category", "Unit")]
public sealed class MessageIdTests
{
    private const string ValidId = "wamid.ABC123";

    [Fact]
    public void Constructor_ValidId_SetsValue()
    {
        var sut = new MessageId(ValidId);

        sut.Value.Should().Be(ValidId);
    }

    [Fact]
    public void Constructor_NullValue_ThrowsArgumentNullException()
    {
        var act = () => new MessageId(null!);

        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("value");
    }

    [Fact]
    public void TryCreate_ValidId_ReturnsTrue()
    {
        var result = MessageId.TryCreate(ValidId, out var messageId);

        result.Should().BeTrue();
        messageId.Value.Should().Be(ValidId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void TryCreate_InvalidInput_ReturnsFalse(string? value)
    {
        var result = MessageId.TryCreate(value, out var messageId);

        result.Should().BeFalse();
        messageId.Should().Be(default(MessageId));
    }

    [Fact]
    public void Create_ValidId_ReturnsMessageId()
    {
        var result = MessageId.Create(ValidId);

        result.Value.Should().Be(ValidId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_InvalidInput_ThrowsArgumentException(string? value)
    {
        var act = () => MessageId.Create(value!);

        act.Should().Throw<ArgumentException>()
            .And.ParamName.Should().Be("value");
    }

    [Fact]
    public void Value_ReturnsStoredValue()
    {
        var sut = new MessageId(ValidId);

        sut.Value.Should().Be(ValidId);
    }

    [Fact]
    public void Equals_SameId_ReturnsTrue()
    {
        var first = new MessageId(ValidId);
        var second = new MessageId(ValidId);

        first.Equals(second).Should().BeTrue();
        (first == second).Should().BeTrue();
        (first != second).Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentId_ReturnsFalse()
    {
        var first = new MessageId("wamid.ABC123");
        var second = new MessageId("wamid.XYZ789");

        first.Equals(second).Should().BeFalse();
        (first == second).Should().BeFalse();
        (first != second).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_SameId_ReturnsSameHash()
    {
        var first = new MessageId(ValidId);
        var second = new MessageId(ValidId);

        first.GetHashCode().Should().Be(second.GetHashCode());
    }

    [Fact]
    public void ImplicitConversion_ToStringAndBack_Works()
    {
        var original = new MessageId(ValidId);

        string asString = original;
        MessageId backToMessageId = asString;

        asString.Should().Be(ValidId);
        backToMessageId.Value.Should().Be(ValidId);
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        var sut = new MessageId(ValidId);

        sut.ToString().Should().Be(ValidId);
    }
}
