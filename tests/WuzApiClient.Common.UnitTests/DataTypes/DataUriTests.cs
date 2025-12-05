using AwesomeAssertions;
using WuzApiClient.Common.DataTypes;

namespace WuzApiClient.Common.UnitTests.DataTypes;

[Trait("Category", "Unit")]
public sealed class DataUriTests
{
    private const string SamplePngBase64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==";
    private const string ValidDataUri = "data:image/png;base64," + SamplePngBase64;

    #region Parse

    [Fact]
    public void Parse_ValidDataUri_ReturnsDataUri()
    {
        var result = DataUri.Parse(ValidDataUri);

        result.MediaType.Should().Be("image/png");
        result.Base64Data.Should().Be(SamplePngBase64);
    }

    [Fact]
    public void Parse_NullValue_ThrowsArgumentNullException()
    {
        var act = () => DataUri.Parse(null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("value");
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid")]
    [InlineData("http://example.com")]
    [InlineData("data:image/png,notbase64")]
    public void Parse_InvalidFormat_ThrowsFormatException(string value)
    {
        var act = () => DataUri.Parse(value);

        act.Should().Throw<FormatException>();
    }

    #endregion

    #region TryParse

    [Fact]
    public void TryParse_ValidDataUri_ReturnsTrue()
    {
        var result = DataUri.TryParse(ValidDataUri, out var dataUri);

        result.Should().BeTrue();
        dataUri!.MediaType.Should().Be("image/png");
        dataUri.Base64Data.Should().Be(SamplePngBase64);
    }

    [Fact]
    public void TryParse_EmptyMediaType_DefaultsToTextPlain()
    {
        var dataUriString = "data:;base64,SGVsbG8gV29ybGQ=";

        var result = DataUri.TryParse(dataUriString, out var dataUri);

        result.Should().BeTrue();
        dataUri!.MediaType.Should().Be("text/plain");
        dataUri.Base64Data.Should().Be("SGVsbG8gV29ybGQ=");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void TryParse_NullOrEmpty_ReturnsFalse(string? value)
    {
        var result = DataUri.TryParse(value!, out var dataUri);

        result.Should().BeFalse();
        dataUri.Should().BeNull();
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("http://example.com")]
    [InlineData("data:image/png,notbase64")]
    public void TryParse_InvalidFormat_ReturnsFalse(string value)
    {
        var result = DataUri.TryParse(value, out var dataUri);

        result.Should().BeFalse();
        dataUri.Should().BeNull();
    }

    [Fact]
    public void TryParse_CaseInsensitiveScheme_ReturnsTrue()
    {
        var dataUriString = "DATA:image/png;BASE64," + SamplePngBase64;

        var result = DataUri.TryParse(dataUriString, out var dataUri);

        result.Should().BeTrue();
        dataUri!.MediaType.Should().Be("image/png");
    }

    #endregion

    #region Create (from base64 string)

    [Fact]
    public void Create_FromBase64String_ReturnsValidDataUri()
    {
        var result = DataUri.Create("image/png", SamplePngBase64);

        result.MediaType.Should().Be("image/png");
        result.Base64Data.Should().Be(SamplePngBase64);
        result.ToString().Should().Be(ValidDataUri);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_NullOrEmptyMediaType_ThrowsArgumentNullException(string? mediaType)
    {
        var act = () => DataUri.Create(mediaType!, "data");

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("mediaType");
    }

    [Fact]
    public void Create_NullBase64Data_ThrowsArgumentNullException()
    {
        var act = () => DataUri.Create("image/png", (string)null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("base64Data");
    }

    [Fact]
    public void Create_EmptyBase64Data_CreatesValidDataUri()
    {
        var result = DataUri.Create("image/png", string.Empty);

        result.MediaType.Should().Be("image/png");
        result.Base64Data.Should().BeEmpty();
    }

    #endregion

    #region Create (from byte array)

    [Fact]
    public void Create_FromBytes_ReturnsValidDataUri()
    {
        var data = new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F };

        var result = DataUri.Create("text/plain", data);

        result.MediaType.Should().Be("text/plain");
        result.Base64Data.Should().Be("SGVsbG8=");
    }

    [Fact]
    public void Create_NullBytes_ThrowsArgumentNullException()
    {
        var act = () => DataUri.Create("image/png", (byte[])null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("data");
    }

    [Fact]
    public void Create_EmptyBytes_CreatesValidDataUri()
    {
        var result = DataUri.Create("image/png", Array.Empty<byte>());

        result.MediaType.Should().Be("image/png");
        result.Base64Data.Should().BeEmpty();
    }

    #endregion

    #region GetBytes

    [Fact]
    public void GetBytes_ValidBase64_ReturnsDecodedBytes()
    {
        var dataUri = DataUri.Create("text/plain", "SGVsbG8=");

        var result = dataUri.GetBytes();

        result.Should().Equal(0x48, 0x65, 0x6C, 0x6C, 0x6F);
    }

    [Fact]
    public void GetBytes_EmptyBase64_ReturnsEmptyArray()
    {
        var dataUri = DataUri.Create("image/png", string.Empty);

        var result = dataUri.GetBytes();

        result.Should().BeEmpty();
    }

    [Fact]
    public void GetBytes_InvalidBase64_ReturnsEmptyArray()
    {
        // Manually construct a DataUri with invalid base64 using reflection or Parse
        // Since we can't create one directly with invalid base64, we test the round-trip
        var dataUri = DataUri.Parse("data:text/plain;base64,!!!invalid!!!");

        var result = dataUri.GetBytes();

        result.Should().BeEmpty();
    }

    [Fact]
    public void GetBytes_RoundTrip_PreservesData()
    {
        var originalData = new byte[] { 0x00, 0x01, 0x02, 0xFF, 0xFE };
        var dataUri = DataUri.Create("application/octet-stream", originalData);

        var result = dataUri.GetBytes();

        result.Should().Equal(originalData);
    }

    #endregion

    #region ToString

    [Fact]
    public void ToString_ReturnsOriginalValue()
    {
        var dataUri = DataUri.Parse(ValidDataUri);

        dataUri.ToString().Should().Be(ValidDataUri);
    }

    [Fact]
    public void ToString_CreatedDataUri_ReturnsFormattedValue()
    {
        var dataUri = DataUri.Create("image/jpeg", "base64data");

        dataUri.ToString().Should().Be("data:image/jpeg;base64,base64data");
    }

    #endregion

    #region Equality

    [Fact]
    public void Equals_SameValue_ReturnsTrue()
    {
        var dataUri1 = DataUri.Parse(ValidDataUri);
        var dataUri2 = DataUri.Parse(ValidDataUri);

        dataUri1.Equals(dataUri2).Should().BeTrue();
        (dataUri1 == dataUri2).Should().BeTrue();
        (dataUri1 != dataUri2).Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ReturnsFalse()
    {
        var dataUri1 = DataUri.Create("image/png", "data1");
        var dataUri2 = DataUri.Create("image/png", "data2");

        dataUri1.Equals(dataUri2).Should().BeFalse();
        (dataUri1 == dataUri2).Should().BeFalse();
        (dataUri1 != dataUri2).Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentMediaType_ReturnsFalse()
    {
        var dataUri1 = DataUri.Create("image/png", "data");
        var dataUri2 = DataUri.Create("image/jpeg", "data");

        dataUri1.Equals(dataUri2).Should().BeFalse();
    }

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        var dataUri = DataUri.Parse(ValidDataUri);

        dataUri.Equals(null).Should().BeFalse();
        (dataUri == null).Should().BeFalse();
        (dataUri != null).Should().BeTrue();
    }

    [Fact]
    public void Equals_SameReference_ReturnsTrue()
    {
        var dataUri = DataUri.Parse(ValidDataUri);

        dataUri.Equals(dataUri).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_SameValue_ReturnsSameHash()
    {
        var dataUri1 = DataUri.Parse(ValidDataUri);
        var dataUri2 = DataUri.Parse(ValidDataUri);

        dataUri1.GetHashCode().Should().Be(dataUri2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentValue_ReturnsDifferentHash()
    {
        var dataUri1 = DataUri.Create("image/png", "data1");
        var dataUri2 = DataUri.Create("image/png", "data2");

        dataUri1.GetHashCode().Should().NotBe(dataUri2.GetHashCode());
    }

    #endregion
}
