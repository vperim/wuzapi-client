using WuzApiClient.Json;
using System.Text.Json;
using AwesomeAssertions;
using WuzApiClient.Common.Serialization;
using WuzApiClient.Common.Models;

namespace WuzApiClient.UnitTests.Json;

/// <summary>
/// Unit tests for <see cref="JidConverter"/>.
/// </summary>
[Trait("Category", "Unit")]
public sealed class JidConverterTests
{
    private readonly JsonSerializerOptions options = WuzApiJsonSerializerOptions.Default;

    [Fact]
    public void Read_ValidJidJson_DeserializesJid()
    {
        var json = "\"5511999999999@s.whatsapp.net\"";

        var jid = JsonSerializer.Deserialize<Jid>(json, this.options);

        jid.Value.Should().Be("5511999999999@s.whatsapp.net");
    }

    [Fact]
    public void Read_NullJidJson_ReturnsDefaultJid()
    {
        var json = "null";

        var jid = JsonSerializer.Deserialize<Jid>(json, this.options);

        jid.Should().Be(default(Jid));
        jid.Value.Should().BeNull();
    }

    [Fact]
    public void Read_EmptyJidJson_ReturnsDefaultJid()
    {
        var json = "\"\"";

        var jid = JsonSerializer.Deserialize<Jid>(json, this.options);

        jid.Should().Be(default(Jid));
        jid.Value.Should().BeNull();
    }

    [Fact]
    public void Write_Jid_SerializesAsString()
    {
        var jid = new Jid("5511999999999@s.whatsapp.net");

        var json = JsonSerializer.Serialize(jid, this.options);

        json.Should().Be("\"5511999999999@s.whatsapp.net\"");
    }

    [Fact]
    public void RoundTrip_Jid_PreservesValue()
    {
        var original = new Jid("5511999999999@s.whatsapp.net");

        var json = JsonSerializer.Serialize(original, this.options);
        var deserialized = JsonSerializer.Deserialize<Jid>(json, this.options);

        deserialized.Should().Be(original);
    }
}
