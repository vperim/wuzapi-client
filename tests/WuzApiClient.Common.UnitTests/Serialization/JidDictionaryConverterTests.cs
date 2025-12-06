using System.Text.Json;
using AwesomeAssertions;
using WuzApiClient.Common.Models;
using WuzApiClient.Common.Serialization;

namespace WuzApiClient.Common.UnitTests.Serialization;

[Trait("Category", "Unit")]
public sealed class JidDictionaryConverterTests
{
    private readonly JsonSerializerOptions _options;

    public JidDictionaryConverterTests()
    {
        _options = new JsonSerializerOptions();
        _options.Converters.Add(new JidDictionaryConverter<string>());
        _options.Converters.Add(new JidConverter());
    }

    #region Deserialization

    [Fact]
    public void Deserialize_NullDictionary_ReturnsNull()
    {
        var json = "null";

        var dict = JsonSerializer.Deserialize<Dictionary<Jid, string>?>(json, _options);

        dict.Should().BeNull();
    }

    [Fact]
    public void Deserialize_EmptyDictionary_ReturnsEmpty()
    {
        var json = "{}";

        var dict = JsonSerializer.Deserialize<Dictionary<Jid, string>?>(json, _options);

        dict.Should().NotBeNull();
        dict!.Count.Should().Be(0);
    }

    [Fact]
    public void Deserialize_ValidDictionary_ReturnsDictionary()
    {
        var json = "{\"5511999999999@s.whatsapp.net\":\"User1\",\"5511888888888@s.whatsapp.net\":\"User2\"}";

        var dict = JsonSerializer.Deserialize<Dictionary<Jid, string>?>(json, _options);

        dict.Should().NotBeNull();
        dict!.Count.Should().Be(2);
        dict[new Jid("5511999999999@s.whatsapp.net")].Should().Be("User1");
        dict[new Jid("5511888888888@s.whatsapp.net")].Should().Be("User2");
    }

    [Fact]
    public void Deserialize_NullValue_PreservesNullValue()
    {
        var json = "{\"5511999999999@s.whatsapp.net\":null}";

        var dict = JsonSerializer.Deserialize<Dictionary<Jid, string>?>(json, _options);

        dict.Should().NotBeNull();
        dict!.Count.Should().Be(1);
        dict.ContainsKey(new Jid("5511999999999@s.whatsapp.net")).Should().BeTrue();
        dict[new Jid("5511999999999@s.whatsapp.net")].Should().BeNull();
    }

    [Fact]
    public void Deserialize_InvalidJidKey_ThrowsJsonException()
    {
        var json = "{\"invalid\":\"value\"}";

        var act = () => JsonSerializer.Deserialize<Dictionary<Jid, string>?>(json, _options);

        act.Should().Throw<JsonException>()
            .WithMessage("*Invalid JID format for dictionary key*");
    }

    [Fact]
    public void Deserialize_EmptyStringKey_ThrowsJsonException()
    {
        var json = "{\"\":\"value\"}";

        var act = () => JsonSerializer.Deserialize<Dictionary<Jid, string>?>(json, _options);

        act.Should().Throw<JsonException>()
            .WithMessage("*Dictionary key cannot be null or empty*");
    }

    [Fact]
    public void Deserialize_NotAnObject_ThrowsJsonException()
    {
        var json = "\"not an object\"";

        var act = () => JsonSerializer.Deserialize<Dictionary<Jid, string>?>(json, _options);

        act.Should().Throw<JsonException>()
            .WithMessage("*Expected object start token*");
    }

    #endregion

    #region Serialization

    [Fact]
    public void Serialize_NullDictionary_WritesNull()
    {
        Dictionary<Jid, string>? dict = null;

        var json = JsonSerializer.Serialize(dict, _options);

        json.Should().Be("null");
    }

    [Fact]
    public void Serialize_EmptyDictionary_WritesEmptyObject()
    {
        var dict = new Dictionary<Jid, string>();

        var json = JsonSerializer.Serialize(dict, _options);

        json.Should().Be("{}");
    }

    [Fact]
    public void Serialize_ValidDictionary_WritesObject()
    {
        var dict = new Dictionary<Jid, string>
        {
            [new Jid("5511999999999@s.whatsapp.net")] = "User1",
            [new Jid("5511888888888@s.whatsapp.net")] = "User2"
        };

        var json = JsonSerializer.Serialize(dict, _options);

        // Note: Dictionary order is not guaranteed, so we need to check both possibilities
        var expected1 = "{\"5511999999999@s.whatsapp.net\":\"User1\",\"5511888888888@s.whatsapp.net\":\"User2\"}";
        var expected2 = "{\"5511888888888@s.whatsapp.net\":\"User2\",\"5511999999999@s.whatsapp.net\":\"User1\"}";

        (json == expected1 || json == expected2).Should().BeTrue();
    }

    [Fact]
    public void Serialize_DictionaryWithNullValue_WritesNullValue()
    {
        var dict = new Dictionary<Jid, string?>
        {
            [new Jid("5511999999999@s.whatsapp.net")] = null
        };

        var options = new JsonSerializerOptions();
        options.Converters.Add(new JidDictionaryConverter<string?>());
        options.Converters.Add(new JidConverter());

        var json = JsonSerializer.Serialize(dict, options);

        json.Should().Be("{\"5511999999999@s.whatsapp.net\":null}");
    }

    [Fact]
    public void Serialize_DictionaryWithNullJidKey_ThrowsJsonException()
    {
        var dict = new Dictionary<Jid, string>
        {
            [default(Jid)] = "value"
        };

        var act = () => JsonSerializer.Serialize(dict, _options);

        act.Should().Throw<JsonException>()
            .WithMessage("*Cannot serialize dictionary with null Jid key*");
    }

    #endregion

    #region Case Sensitivity

    [Fact]
    public void Dictionary_CaseInsensitiveKeys_TreatAsSameKey()
    {
        var dict = new Dictionary<Jid, string>();
        var jid1 = new Jid("5511999999999@s.whatsapp.net");
        var jid2 = new Jid("5511999999999@S.WHATSAPP.NET"); // Different case

        dict[jid1] = "Value1";
        dict[jid2] = "Value2"; // Should overwrite

        dict.Count.Should().Be(1);
        dict[jid1].Should().Be("Value2");
        dict[jid2].Should().Be("Value2");
    }

    #endregion

    #region HashCode Consistency

    [Fact]
    public void Dictionary_AsHashKey_ConsistentHashCode()
    {
        var jid1 = new Jid("5511999999999@s.whatsapp.net");
        var jid2 = new Jid("5511999999999@S.WHATSAPP.NET");

        var hash1 = jid1.GetHashCode();
        var hash2 = jid2.GetHashCode();

        hash1.Should().Be(hash2);
    }

    [Fact]
    public void Dictionary_JidAsKey_LookupWorks()
    {
        var dict = new Dictionary<Jid, string>
        {
            [new Jid("5511999999999@s.whatsapp.net")] = "User1"
        };

        var lookupJid = new Jid("5511999999999@S.WHATSAPP.NET"); // Different case

        dict.ContainsKey(lookupJid).Should().BeTrue();
        dict[lookupJid].Should().Be("User1");
    }

    #endregion

    #region Round-Trip

    [Fact]
    public void RoundTrip_Dictionary_PreservesValues()
    {
        var original = new Dictionary<Jid, string>
        {
            [new Jid("5511999999999@s.whatsapp.net")] = "User1",
            [new Jid("120363123456789012@g.us")] = "Group1"
        };

        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<Dictionary<Jid, string>?>(json, _options);

        deserialized.Should().NotBeNull();
        deserialized!.Count.Should().Be(2);
        deserialized[new Jid("5511999999999@s.whatsapp.net")].Should().Be("User1");
        deserialized[new Jid("120363123456789012@g.us")].Should().Be("Group1");
    }

    [Fact]
    public void RoundTrip_EmptyDictionary_PreservesEmpty()
    {
        var original = new Dictionary<Jid, string>();

        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<Dictionary<Jid, string>?>(json, _options);

        deserialized.Should().NotBeNull();
        deserialized!.Count.Should().Be(0);
    }

    #endregion

    #region Complex Value Types

    [Fact]
    public void Deserialize_DictionaryWithComplexValues_Works()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new JidDictionaryConverter<UserInfo>());
        options.Converters.Add(new JidConverter());

        var json = "{\"5511999999999@s.whatsapp.net\":{\"Name\":\"John\"}}";

        var dict = JsonSerializer.Deserialize<Dictionary<Jid, UserInfo>?>(json, options);

        dict.Should().NotBeNull();
        dict!.Count.Should().Be(1);
        dict[new Jid("5511999999999@s.whatsapp.net")].Name.Should().Be("John");
    }

    private class UserInfo
    {
        public string? Name { get; set; }
    }

    #endregion
}
