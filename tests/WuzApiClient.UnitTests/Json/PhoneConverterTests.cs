using WuzApiClient.Json;
using System.Text.Json;
using AwesomeAssertions;
using WuzApiClient.Common.Serialization;
using WuzApiClient.Common.Models;

namespace WuzApiClient.UnitTests.Json;

/// <summary>
/// Unit tests for <see cref="PhoneConverter"/>.
/// </summary>
[Trait("Category", "Unit")]
public sealed class PhoneConverterTests
{
    private readonly JsonSerializerOptions options = WuzApiJsonSerializerOptions.Default;

    [Fact]
    public void Read_ValidPhoneJson_DeserializesPhone()
    {
        var json = "\"5511999999999\"";

        var phone = JsonSerializer.Deserialize<Phone>(json, this.options);

        phone.Value.Should().Be("5511999999999");
    }

    [Fact]
    public void Read_InvalidPhoneJson_ThrowsJsonException()
    {
        var json = "\"invalid\"";

        var act = () => JsonSerializer.Deserialize<Phone>(json, this.options);

        act.Should().Throw<JsonException>()
            .WithMessage("*Invalid phone number format*");
    }

    [Fact]
    public void Read_NullPhoneJson_ThrowsJsonException()
    {
        var json = "null";

        var act = () => JsonSerializer.Deserialize<Phone>(json, this.options);

        act.Should().Throw<JsonException>()
            .WithMessage("*Invalid phone number format*");
    }

    [Fact]
    public void Write_Phone_SerializesAsString()
    {
        var phone = Phone.Create("5511999999999");

        var json = JsonSerializer.Serialize(phone, this.options);

        json.Should().Be("\"5511999999999\"");
    }

    [Fact]
    public void RoundTrip_Phone_PreservesValue()
    {
        var original = Phone.Create("5511999999999");

        var json = JsonSerializer.Serialize(original, this.options);
        var deserialized = JsonSerializer.Deserialize<Phone>(json, this.options);

        deserialized.Should().Be(original);
    }
}
