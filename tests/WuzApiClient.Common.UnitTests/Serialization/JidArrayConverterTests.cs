using System.Text.Json;
using AwesomeAssertions;
using WuzApiClient.Common.Models;
using WuzApiClient.Common.Serialization;

namespace WuzApiClient.Common.UnitTests.Serialization;

[Trait("Category", "Unit")]
public sealed class JidArrayConverterTests
{
    private readonly JsonSerializerOptions _options;

    public JidArrayConverterTests()
    {
        _options = new JsonSerializerOptions();
        _options.Converters.Add(new JidArrayConverter());
        _options.Converters.Add(new JidConverter());
    }

    #region Deserialization

    [Fact]
    public void Deserialize_NullArray_ReturnsNull()
    {
        var json = "null";

        var array = JsonSerializer.Deserialize<Jid[]?>(json, _options);

        array.Should().BeNull();
    }

    [Fact]
    public void Deserialize_EmptyArray_ReturnsEmptyArray()
    {
        var json = "[]";

        var array = JsonSerializer.Deserialize<Jid[]?>(json, _options);

        array.Should().NotBeNull();
        array!.Length.Should().Be(0);
    }

    [Fact]
    public void Deserialize_ValidJidArray_ReturnsJidArray()
    {
        var json = "[\"5511999999999@s.whatsapp.net\",\"5511888888888@s.whatsapp.net\"]";

        var array = JsonSerializer.Deserialize<Jid[]?>(json, _options);

        array.Should().NotBeNull();
        array!.Length.Should().Be(2);
        array[0].Value.Should().Be("5511999999999@s.whatsapp.net");
        array[1].Value.Should().Be("5511888888888@s.whatsapp.net");
    }

    [Fact]
    public void Deserialize_InvalidJidInArray_ThrowsJsonException()
    {
        var json = "[\"5511999999999@s.whatsapp.net\",\"invalid\"]";

        var act = () => JsonSerializer.Deserialize<Jid[]?>(json, _options);

        act.Should().Throw<JsonException>()
            .WithMessage("*Invalid JID format in array*");
    }

    [Fact]
    public void Deserialize_EmptyStringInArray_ThrowsJsonException()
    {
        var json = "[\"5511999999999@s.whatsapp.net\",\"\"]";

        var act = () => JsonSerializer.Deserialize<Jid[]?>(json, _options);

        act.Should().Throw<JsonException>()
            .WithMessage("*Array contains null or empty JID value*");
    }

    [Fact]
    public void Deserialize_NullElementInArray_AddsDefaultJid()
    {
        var json = "[\"5511999999999@s.whatsapp.net\",null]";

        var array = JsonSerializer.Deserialize<Jid[]?>(json, _options);

        array.Should().NotBeNull();
        array!.Length.Should().Be(2);
        array[0].Value.Should().Be("5511999999999@s.whatsapp.net");
        array[1].Should().Be(default(Jid));
        array[1].Value.Should().BeNull();
    }

    [Fact]
    public void Deserialize_NotAnArray_ThrowsJsonException()
    {
        var json = "\"not an array\"";

        var act = () => JsonSerializer.Deserialize<Jid[]?>(json, _options);

        act.Should().Throw<JsonException>()
            .WithMessage("*Expected array start token*");
    }

    #endregion

    #region Serialization

    [Fact]
    public void Serialize_NullArray_WritesNull()
    {
        Jid[]? array = null;

        var json = JsonSerializer.Serialize(array, _options);

        json.Should().Be("null");
    }

    [Fact]
    public void Serialize_EmptyArray_WritesEmptyArray()
    {
        var array = Array.Empty<Jid>();

        var json = JsonSerializer.Serialize(array, _options);

        json.Should().Be("[]");
    }

    [Fact]
    public void Serialize_ValidArray_WritesStringArray()
    {
        var array = new[]
        {
            new Jid("5511999999999@s.whatsapp.net"),
            new Jid("5511888888888@s.whatsapp.net")
        };

        var json = JsonSerializer.Serialize(array, _options);

        json.Should().Be("[\"5511999999999@s.whatsapp.net\",\"5511888888888@s.whatsapp.net\"]");
    }

    [Fact]
    public void Serialize_ArrayWithDefaultJid_WritesNull()
    {
        var array = new[]
        {
            new Jid("5511999999999@s.whatsapp.net"),
            default(Jid)
        };

        var json = JsonSerializer.Serialize(array, _options);

        json.Should().Be("[\"5511999999999@s.whatsapp.net\",null]");
    }

    #endregion

    #region Round-Trip

    [Fact]
    public void RoundTrip_Array_PreservesValues()
    {
        var original = new[]
        {
            new Jid("5511999999999@s.whatsapp.net"),
            new Jid("120363123456789012@g.us")
        };

        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<Jid[]?>(json, _options);

        deserialized.Should().NotBeNull();
        deserialized!.Length.Should().Be(2);
        deserialized[0].Value.Should().Be("5511999999999@s.whatsapp.net");
        deserialized[1].Value.Should().Be("120363123456789012@g.us");
    }

    [Fact]
    public void RoundTrip_EmptyArray_PreservesEmpty()
    {
        var original = Array.Empty<Jid>();

        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<Jid[]?>(json, _options);

        deserialized.Should().NotBeNull();
        deserialized!.Length.Should().Be(0);
    }

    #endregion

    #region Complex Objects

    [Fact]
    public void Deserialize_ObjectWithJidArrayProperty_Works()
    {
        var json = "{\"Jids\":[\"5511999999999@s.whatsapp.net\",\"5511888888888@s.whatsapp.net\"]}";

        var obj = JsonSerializer.Deserialize<TestObject>(json, _options);

        obj.Should().NotBeNull();
        obj!.Jids.Should().NotBeNull();
        obj.Jids!.Length.Should().Be(2);
    }

    private class TestObject
    {
        public Jid[]? Jids { get; set; }
    }

    #endregion
}
