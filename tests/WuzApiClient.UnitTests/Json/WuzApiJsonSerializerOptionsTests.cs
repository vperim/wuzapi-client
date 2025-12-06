using WuzApiClient.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using AwesomeAssertions;
using WuzApiClient.Common.Serialization;

namespace WuzApiClient.UnitTests.Json;

[Trait("Category", "Unit")]
public sealed class WuzApiJsonSerializerOptionsTests
{
    [Fact]
    public void Default_ReturnsSameInstance()
    {
        // Act
        var options1 = WuzApiJsonSerializerOptions.Default;
        var options2 = WuzApiJsonSerializerOptions.Default;

        // Assert
        options1.Should().BeSameAs(options2);
    }

    [Fact]
    public void Default_PropertyNameCaseInsensitive_IsTrue()
    {
        // Act
        var options = WuzApiJsonSerializerOptions.Default;

        // Assert
        options.PropertyNameCaseInsensitive.Should().BeTrue();
    }

    [Fact]
    public void Default_PropertyNamingPolicy_IsCamelCase()
    {
        // Act
        var options = WuzApiJsonSerializerOptions.Default;

        // Assert
        options.PropertyNamingPolicy.Should().Be(JsonNamingPolicy.CamelCase);
    }

    [Fact]
    public void Default_DefaultIgnoreCondition_WhenWritingNull()
    {
        // Act
        var options = WuzApiJsonSerializerOptions.Default;

        // Assert
        options.DefaultIgnoreCondition.Should().Be(JsonIgnoreCondition.WhenWritingNull);
    }

    [Fact]
    public void Default_NumberHandling_AllowsReadingFromString()
    {
        // Act
        var options = WuzApiJsonSerializerOptions.Default;

        // Assert
        options.NumberHandling.Should().Be(JsonNumberHandling.AllowReadingFromString);
    }

    [Fact]
    public void Default_HasPhoneConverter()
    {
        // Act
        var options = WuzApiJsonSerializerOptions.Default;

        // Assert
        options.Converters.Should().Contain(c => c.GetType() == typeof(PhoneConverter));
    }

    [Fact]
    public void Default_HasJidConverter()
    {
        // Act
        var options = WuzApiJsonSerializerOptions.Default;

        // Assert
        options.Converters.Should().Contain(c => c.GetType() == typeof(JidConverter));
    }

    [Fact]
    public void Default_HasStringEnumConverter()
    {
        // Act
        var options = WuzApiJsonSerializerOptions.Default;

        // Assert
        options.Converters.Should().Contain(c => c.GetType() == typeof(JsonStringEnumConverter));
    }
}
