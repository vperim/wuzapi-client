using System.Text.Json;
using AwesomeAssertions;
using WuzApiClient.Common.DataTypes;
using WuzApiClient.Common.Serialization;

namespace WuzApiClient.Common.UnitTests.Serialization;

[Trait("Category", "Unit")]
public sealed class DataUriJsonConverterTests
{
    private const string SamplePngBase64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==";
    private const string ValidDataUri = "data:image/png;base64," + SamplePngBase64;

    private readonly JsonSerializerOptions options;

    public DataUriJsonConverterTests()
    {
        this.options = new JsonSerializerOptions();
        this.options.Converters.Add(new DataUriJsonConverter());
    }

    #region Read

    [Fact]
    public void Read_ValidDataUri_DeserializesDataUri()
    {
        var json = $"\"{ValidDataUri}\"";

        var result = JsonSerializer.Deserialize<DataUri?>(json, this.options);

        result.Should().NotBeNull();
        result!.MediaType.Should().Be("image/png");
        result.Base64Data.Should().Be(SamplePngBase64);
    }

    [Fact]
    public void Read_NullValue_ReturnsNull()
    {
        var json = "null";

        var result = JsonSerializer.Deserialize<DataUri?>(json, this.options);

        result.Should().BeNull();
    }

    [Fact]
    public void Read_EmptyString_ReturnsNull()
    {
        var json = "\"\"";

        var result = JsonSerializer.Deserialize<DataUri?>(json, this.options);

        result.Should().BeNull();
    }

    [Fact]
    public void Read_InvalidDataUri_ReturnsNull()
    {
        var json = "\"not-a-data-uri\"";

        var result = JsonSerializer.Deserialize<DataUri?>(json, this.options);

        result.Should().BeNull();
    }

    [Fact]
    public void Read_HttpUrl_ReturnsNull()
    {
        var json = "\"https://example.com/image.png\"";

        var result = JsonSerializer.Deserialize<DataUri?>(json, this.options);

        result.Should().BeNull();
    }

    #endregion

    #region Write

    [Fact]
    public void Write_DataUri_SerializesAsString()
    {
        var dataUri = DataUri.Parse(ValidDataUri);

        var json = JsonSerializer.Serialize(dataUri, this.options);

        // JSON may escape + as \u002B, so deserialize to compare
        var deserialized = JsonSerializer.Deserialize<string>(json);
        deserialized.Should().Be(ValidDataUri);
    }

    [Fact]
    public void Write_NullDataUri_SerializesAsNull()
    {
        DataUri? dataUri = null;

        var json = JsonSerializer.Serialize(dataUri, this.options);

        json.Should().Be("null");
    }

    #endregion

    #region RoundTrip

    [Fact]
    public void RoundTrip_DataUri_PreservesValue()
    {
        var original = DataUri.Parse(ValidDataUri);

        var json = JsonSerializer.Serialize(original, this.options);
        var deserialized = JsonSerializer.Deserialize<DataUri?>(json, this.options);

        deserialized.Should().NotBeNull();
        deserialized!.ToString().Should().Be(original.ToString());
        deserialized.MediaType.Should().Be(original.MediaType);
        deserialized.Base64Data.Should().Be(original.Base64Data);
    }

    [Fact]
    public void RoundTrip_NullDataUri_PreservesNull()
    {
        DataUri? original = null;

        var json = JsonSerializer.Serialize(original, this.options);
        var deserialized = JsonSerializer.Deserialize<DataUri?>(json, this.options);

        deserialized.Should().BeNull();
    }

    #endregion

    #region Object Serialization

    private sealed record TestRecord
    {
        [System.Text.Json.Serialization.JsonConverter(typeof(DataUriJsonConverter))]
        public DataUri? Image { get; init; }

        public string? Name { get; init; }
    }

    [Fact]
    public void Deserialize_ObjectWithDataUri_Works()
    {
        var json = $"{{\"Image\":\"{ValidDataUri}\",\"Name\":\"test\"}}";

        var result = JsonSerializer.Deserialize<TestRecord>(json, this.options);

        result.Should().NotBeNull();
        result!.Image.Should().NotBeNull();
        result.Image!.MediaType.Should().Be("image/png");
        result.Name.Should().Be("test");
    }

    [Fact]
    public void Deserialize_ObjectWithNullDataUri_Works()
    {
        var json = "{\"Image\":null,\"Name\":\"test\"}";

        var result = JsonSerializer.Deserialize<TestRecord>(json, this.options);

        result.Should().NotBeNull();
        result!.Image.Should().BeNull();
        result.Name.Should().Be("test");
    }

    [Fact]
    public void Serialize_ObjectWithDataUri_Works()
    {
        var record = new TestRecord
        {
            Image = DataUri.Parse(ValidDataUri),
            Name = "test"
        };

        var json = JsonSerializer.Serialize(record, this.options);
        var deserialized = JsonSerializer.Deserialize<TestRecord>(json, this.options);

        deserialized.Should().NotBeNull();
        deserialized!.Image.Should().NotBeNull();
        deserialized.Image!.ToString().Should().Be(ValidDataUri);
        deserialized.Name.Should().Be("test");
    }

    [Fact]
    public void Serialize_ObjectWithNullDataUri_Works()
    {
        var record = new TestRecord
        {
            Image = null,
            Name = "test"
        };

        var json = JsonSerializer.Serialize(record, this.options);

        json.Should().Contain("\"Image\":null");
        json.Should().Contain("\"Name\":\"test\"");
    }

    #endregion
}
