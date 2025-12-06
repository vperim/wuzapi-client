using AwesomeAssertions;
using WuzApiClient.Common.Models;

namespace WuzApiClient.Common.UnitTests.Models;

[Trait("Category", "Unit")]
public sealed class JidTests
{
    private const string UserSuffix = "@s.whatsapp.net";
    private const string GroupSuffix = "@g.us";

    #region Constructor and Nullability

    [Fact]
    public void Constructor_ValidJid_SetsValue()
    {
        var jid = new Jid("5511999999999@s.whatsapp.net");

        jid.Value.Should().Be("5511999999999@s.whatsapp.net");
    }

    [Fact]
    public void Constructor_NullValue_AllowsNull()
    {
        var jid = new Jid(null);

        jid.Value.Should().BeNull();
    }

    [Fact]
    public void DefaultStruct_HasNullValue()
    {
        var jid = default(Jid);

        jid.Value.Should().BeNull();
    }

    [Fact]
    public void DefaultStruct_IsUserReturnsFalse()
    {
        var jid = default(Jid);

        jid.IsUser.Should().BeFalse();
    }

    [Fact]
    public void DefaultStruct_IsGroupReturnsFalse()
    {
        var jid = default(Jid);

        jid.IsGroup.Should().BeFalse();
    }

    [Fact]
    public void DefaultStruct_NumberReturnsEmpty()
    {
        var jid = default(Jid);

        jid.Number.Should().Be(string.Empty);
    }

    [Fact]
    public void DefaultStruct_ToStringReturnsEmpty()
    {
        var jid = default(Jid);

        jid.ToString().Should().Be(string.Empty);
    }

    [Fact]
    public void NullableJid_CanBeNull()
    {
        Jid? nullableJid = null;

        nullableJid.Should().BeNull();
    }

    #endregion

    #region IsUser / IsGroup

    [Theory]
    [InlineData("5511999999999@s.whatsapp.net", true)]
    [InlineData("5511999999999@S.WHATSAPP.NET", true)]
    [InlineData("5511999999999@g.us", false)]
    [InlineData("5511999999999", false)]
    public void IsUser_VariousJids_ReturnsExpected(string value, bool expected)
    {
        var jid = new Jid(value);

        jid.IsUser.Should().Be(expected);
    }

    [Theory]
    [InlineData("120363123456789012@g.us", true)]
    [InlineData("120363123456789012@G.US", true)]
    [InlineData("5511999999999@s.whatsapp.net", false)]
    [InlineData("120363123456789012", false)]
    public void IsGroup_VariousJids_ReturnsExpected(string value, bool expected)
    {
        var jid = new Jid(value);

        jid.IsGroup.Should().Be(expected);
    }

    #endregion

    #region Number

    [Theory]
    [InlineData("5511999999999@s.whatsapp.net", "5511999999999")]
    [InlineData("120363123456789012@g.us", "120363123456789012")]
    [InlineData("noatsign", "noatsign")]
    public void Number_VariousJids_ExtractsCorrectly(string value, string expected)
    {
        var jid = new Jid(value);

        jid.Number.Should().Be(expected);
    }

    [Fact]
    public void Number_EmptyValue_ReturnsEmpty()
    {
        var jid = new Jid(string.Empty);

        jid.Number.Should().BeEmpty();
    }

    #endregion

    #region FromPhone / FromPhoneNumber

    [Fact]
    public void FromPhone_CreatesUserJid()
    {
        var phone = Phone.Create("5511999999999");

        var jid = Jid.FromPhone(phone);

        jid.Value.Should().Be("5511999999999@s.whatsapp.net");
        jid.IsUser.Should().BeTrue();
    }

    [Fact]
    public void FromPhoneNumber_CreatesUserJid()
    {
        var jid = Jid.FromPhoneNumber("5511999999999");

        jid.Value.Should().Be("5511999999999@s.whatsapp.net");
        jid.IsUser.Should().BeTrue();
    }

    #endregion

    #region FromGroupId

    [Theory]
    [InlineData("120363123456789012@g.us", "120363123456789012@g.us")]
    [InlineData("120363123456789012@G.US", "120363123456789012@G.US")]
    [InlineData("120363123456789012", "120363123456789012@g.us")]
    public void FromGroupId_VariousInputs_ReturnsCorrectJid(string input, string expected)
    {
        var jid = Jid.FromGroupId(input);

        jid.Value.Should().Be(expected);
        jid.IsGroup.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void FromGroupId_NullOrEmpty_ThrowsArgumentException(string? groupId)
    {
        var act = () => Jid.FromGroupId(groupId!);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("groupId");
    }

    #endregion

    #region TryParse

    [Theory]
    [InlineData("5511999999999@s.whatsapp.net", true)]
    [InlineData("120363123456789012@g.us", true)]
    [InlineData("anything@domain", true)]
    public void TryParse_ValidJid_ReturnsTrue(string value, bool _)
    {
        var result = Jid.TryParse(value, out var jid);

        result.Should().BeTrue();
        jid.Value.Should().Be(value);
    }

    [Theory]
    [InlineData("noatsign")]
    [InlineData("plaintext")]
    public void TryParse_InvalidJid_ReturnsFalse(string value)
    {
        var result = Jid.TryParse(value, out var jid);

        result.Should().BeFalse();
        jid.Value.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void TryParse_NullOrEmpty_ReturnsFalse(string? value)
    {
        var result = Jid.TryParse(value, out var jid);

        result.Should().BeFalse();
        jid.Value.Should().BeNull();
    }

    #endregion

    #region Parse

    [Fact]
    public void Parse_ValidJid_ReturnsJid()
    {
        var jid = Jid.Parse("5511999999999@s.whatsapp.net");

        jid.Value.Should().Be("5511999999999@s.whatsapp.net");
    }

    [Theory]
    [InlineData("noatsign")]
    [InlineData(null)]
    [InlineData("")]
    public void Parse_InvalidJid_ThrowsArgumentException(string? value)
    {
        var act = () => Jid.Parse(value!);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("value");
    }

    #endregion

    #region Equality

    [Theory]
    [InlineData("5511999999999@s.whatsapp.net", "5511999999999@s.whatsapp.net", true)]
    [InlineData("5511999999999@S.WHATSAPP.NET", "5511999999999@s.whatsapp.net", true)]
    [InlineData("5511999999999@s.whatsapp.net", "5511888888888@s.whatsapp.net", false)]
    [InlineData("5511999999999@s.whatsapp.net", "5511999999999@g.us", false)]
    public void Equals_VariousJids_ReturnsExpected(string value1, string value2, bool expected)
    {
        var jid1 = new Jid(value1);
        var jid2 = new Jid(value2);

        jid1.Equals(jid2).Should().Be(expected);
        (jid1 == jid2).Should().Be(expected);
        (jid1 != jid2).Should().Be(!expected);
    }

    [Fact]
    public void GetHashCode_SameJidsDifferentCase_ReturnsSameHash()
    {
        var jid1 = new Jid("5511999999999@s.whatsapp.net");
        var jid2 = new Jid("5511999999999@S.WHATSAPP.NET");

        jid1.GetHashCode().Should().Be(jid2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DefaultJid_ReturnsZero()
    {
        var jid = default(Jid);

        jid.GetHashCode().Should().Be(0);
    }

    #endregion

    #region Conversions

    [Fact]
    public void ImplicitConversion_ToStringAndBack_Works()
    {
        var original = new Jid("5511999999999@s.whatsapp.net");

        string? asString = original;
        Jid backToJid = asString;

        asString.Should().Be("5511999999999@s.whatsapp.net");
        backToJid.Value.Should().Be(original.Value);
    }

    [Fact]
    public void ImplicitConversion_NullString_CreatesDefaultJid()
    {
        string? nullString = null;
        Jid jid = nullString;

        jid.Value.Should().BeNull();
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        var jid = new Jid("5511999999999@s.whatsapp.net");

        jid.ToString().Should().Be("5511999999999@s.whatsapp.net");
    }

    #endregion

    #region Dictionary HashCode Consistency

    [Fact]
    public void Dictionary_JidAsKey_WorksCorrectly()
    {
        var dict = new Dictionary<Jid, string>();
        var jid1 = new Jid("5511999999999@s.whatsapp.net");
        var jid2 = new Jid("5511999999999@S.WHATSAPP.NET"); // Different case

        dict[jid1] = "value1";
        dict[jid2] = "value2"; // Should overwrite due to case-insensitive equality

        dict.Count.Should().Be(1);
        dict[jid1].Should().Be("value2");
    }

    [Fact]
    public void Dictionary_JidAsKey_ConsistentHashCode()
    {
        var jid1 = new Jid("5511999999999@s.whatsapp.net");
        var jid2 = new Jid("5511999999999@S.WHATSAPP.NET");

        var hash1 = jid1.GetHashCode();
        var hash2 = jid2.GetHashCode();

        hash1.Should().Be(hash2);
    }

    #endregion
}
