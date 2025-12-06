using AwesomeAssertions;
using WuzApiClient.Common.Models;

namespace WuzApiClient.UnitTests.Models.Common;

[Trait("Category", "Unit")]
public sealed class PhoneTests
{
    [Theory]
    [InlineData("5511999999999", true, "5511999999999")]
    [InlineData("+5511999999999", true, "5511999999999")]
    [InlineData("55-11-99999-9999", true, "5511999999999")]
    [InlineData("55 11 99999 9999", true, "5511999999999")]
    [InlineData("(55) 11 99999-9999", true, "5511999999999")]
    [InlineData("123456789", false, null)]
    [InlineData("1234567890123456", false, null)]
    [InlineData("", false, null)]
    [InlineData(null, false, null)]
    [InlineData("   ", false, null)]
    [InlineData("55ABC999", false, null)]
    public void TryCreate_VariousInputs_ReturnsExpected(string? input, bool expectedSuccess, string? expectedValue)
    {
        var result = Phone.TryCreate(input, out var phone);

        result.Should().Be(expectedSuccess);
        if (expectedSuccess)
        {
            phone.Value.Should().Be(expectedValue);
        }
    }

    [Fact]
    public void Create_ValidNumber_ReturnsPhone()
    {
        var phone = Phone.Create("5511999999999");

        phone.Value.Should().Be("5511999999999");
    }

    [Fact]
    public void Create_InvalidNumber_ThrowsArgumentException()
    {
        var act = () => Phone.Create("invalid");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("value")
            .WithMessage("*Invalid phone number format*");
    }

    [Fact]
    public void ToJid_ReturnsWhatsAppJidFormat()
    {
        var phone = Phone.Create("5511999999999");

        var jid = Jid.FromPhone(phone);

        jid.Value.Should().Be("5511999999999@s.whatsapp.net");
        jid.IsUser.Should().BeTrue();
    }

    [Fact]
    public void Equals_SameNumber_ReturnsTrue()
    {
        var phone1 = Phone.Create("5511999999999");
        var phone2 = Phone.Create("5511999999999");

        phone1.Equals(phone2).Should().BeTrue();
        (phone1 == phone2).Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentNumber_ReturnsFalse()
    {
        var phone1 = Phone.Create("5511999999999");
        var phone2 = Phone.Create("5521888888888");

        phone1.Equals(phone2).Should().BeFalse();
        (phone1 != phone2).Should().BeTrue();
    }

    [Fact]
    public void ImplicitConversion_ToString_ReturnsValue()
    {
        var phone = Phone.Create("5511999999999");

        string value = phone;

        value.Should().Be("5511999999999");
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        var phone = Phone.Create("5511999999999");

        phone.ToString().Should().Be("5511999999999");
    }
}
