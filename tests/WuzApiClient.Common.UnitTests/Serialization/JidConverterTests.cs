using System.Text.Json;
using AwesomeAssertions;
using WuzApiClient.Common.Models;
using WuzApiClient.Common.Serialization;

namespace WuzApiClient.Common.UnitTests.Serialization;

[Trait("Category", "Unit")]
public sealed class JidConverterTests
{
    private readonly JsonSerializerOptions _options;

    public JidConverterTests()
    {
        _options = new JsonSerializerOptions();
        _options.Converters.Add(new JidConverter());
    }

    #region Deserialization

    [Fact]
    public void Deserialize_ValidJid_ReturnsJid()
    {
        var json = "\"5511999999999@s.whatsapp.net\"";

        var jid = JsonSerializer.Deserialize<Jid>(json, _options);

        jid.Value.Should().Be("5511999999999@s.whatsapp.net");
    }

    [Fact]
    public void Deserialize_NullToken_ReturnsDefaultJid()
    {
        var json = "null";

        var jid = JsonSerializer.Deserialize<Jid>(json, _options);

        jid.Should().Be(default(Jid));
        jid.Value.Should().BeNull();
    }

    [Fact]
    public void Deserialize_EmptyString_ReturnsDefault()
    {
        var json = "\"\"";

        var result = JsonSerializer.Deserialize<Jid>(json, _options);

        result.Should().Be(default(Jid));
        result.Value.Should().BeNull();
    }

    [Fact]
    public void Deserialize_NullableJid_HandlesNull()
    {
        var json = "null";

        var jid = JsonSerializer.Deserialize<Jid?>(json, _options);

        jid.Should().BeNull();
    }

    #endregion

    #region Serialization

    [Fact]
    public void Serialize_ValidJid_WritesString()
    {
        var jid = new Jid("5511999999999@s.whatsapp.net");

        var json = JsonSerializer.Serialize(jid, _options);

        json.Should().Be("\"5511999999999@s.whatsapp.net\"");
    }

    [Fact]
    public void Serialize_DefaultJid_WritesNull()
    {
        var jid = default(Jid);

        var json = JsonSerializer.Serialize(jid, _options);

        json.Should().Be("null");
    }

    [Fact]
    public void Serialize_JidWithNullValue_WritesNull()
    {
        var jid = new Jid(null);

        var json = JsonSerializer.Serialize(jid, _options);

        json.Should().Be("null");
    }

    [Fact]
    public void Serialize_NullableJidNull_WritesNull()
    {
        Jid? jid = null;

        var json = JsonSerializer.Serialize(jid, _options);

        json.Should().Be("null");
    }

    #endregion

    #region Round-Trip

    [Fact]
    public void RoundTrip_ValidJid_PreservesValue()
    {
        var original = new Jid("5511999999999@s.whatsapp.net");

        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<Jid>(json, _options);

        deserialized.Value.Should().Be(original.Value);
    }

    [Fact]
    public void RoundTrip_DefaultJid_PreservesDefault()
    {
        var original = default(Jid);

        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<Jid>(json, _options);

        deserialized.Should().Be(default(Jid));
        deserialized.Value.Should().BeNull();
    }

    #endregion

    #region Complex Objects

    [Fact]
    public void Deserialize_ObjectWithJidProperty_Works()
    {
        var json = "{\"Jid\":\"5511999999999@s.whatsapp.net\"}";

        var obj = JsonSerializer.Deserialize<TestObject>(json, _options);

        obj.Should().NotBeNull();
        obj!.Jid.Value.Should().Be("5511999999999@s.whatsapp.net");
    }

    [Fact]
    public void Deserialize_ObjectWithNullJid_Works()
    {
        var json = "{\"Jid\":null}";

        var obj = JsonSerializer.Deserialize<TestObject>(json, _options);

        obj.Should().NotBeNull();
        obj!.Jid.Value.Should().BeNull();
    }

    private class TestObject
    {
        public Jid Jid { get; set; }
    }

    #endregion
}
