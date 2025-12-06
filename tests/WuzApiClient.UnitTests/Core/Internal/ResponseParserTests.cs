using WuzApiClient.Json;
using AwesomeAssertions;
using WuzApiClient.Common.Results;
using WuzApiClient.Core.Internal;
using WuzApiClient.Common.Serialization;

namespace WuzApiClient.UnitTests.Core.Internal;

[Trait("Category", "Unit")]
public sealed class ResponseParserTests
{
    private sealed class TestResponse
    {
        public string? Name { get; set; }
        public int Value { get; set; }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t\n")]
    public void Parse_EmptyOrNullOrWhitespaceContent_ReturnsDeserializationError(string? content)
    {
        var result = ResponseParser.Parse<TestResponse>(content!, WuzApiJsonSerializerOptions.Default);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(WuzApiErrorCode.DeserializationError);
        result.Error.Message.Should().Contain("empty");
    }

    [Fact]
    public void Parse_ResponseWithDataWrapper_ExtractsData()
    {
        const string json = """{"data": {"name": "Test", "value": 42}}""";

        var result = ResponseParser.Parse<TestResponse>(json, WuzApiJsonSerializerOptions.Default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Test");
        result.Value.Value.Should().Be(42);
    }

    [Fact]
    public void Parse_ResponseWithPascalCaseDataWrapper_ExtractsData()
    {
        const string json = """{"Data": {"name": "Test", "value": 42}}""";

        var result = ResponseParser.Parse<TestResponse>(json, WuzApiJsonSerializerOptions.Default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Test");
        result.Value.Value.Should().Be(42);
    }

    [Fact]
    public void Parse_DirectResponse_DeserializesDirectly()
    {
        const string json = """{"name": "Direct", "value": 99}""";

        var result = ResponseParser.Parse<TestResponse>(json, WuzApiJsonSerializerOptions.Default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Direct");
        result.Value.Value.Should().Be(99);
    }

    [Fact]
    public void Parse_NullDataValue_FallsBackToDirectDeserialization()
    {
        // When data property is null, the parser falls through to direct deserialization
        // which creates a default object (Name=null, Value=0)
        const string json = """{"data": null}""";

        var result = ResponseParser.Parse<TestResponse>(json, WuzApiJsonSerializerOptions.Default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().BeNull();
        result.Value.Value.Should().Be(0);
    }

    [Fact]
    public void Parse_InvalidJson_ReturnsDeserializationError()
    {
        const string json = """{"name": "broken""";

        var result = ResponseParser.Parse<TestResponse>(json, WuzApiJsonSerializerOptions.Default);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(WuzApiErrorCode.DeserializationError);
        result.Error.Message.Should().Contain("JSON");
    }

    [Theory]
    [InlineData("""{"error": "Something went wrong"}""", "Something went wrong")]
    [InlineData("""{"Error": "PascalCase error"}""", "PascalCase error")]
    [InlineData("""{"message": "Message prop"}""", "Message prop")]
    [InlineData("""{"Message": "PascalCase message"}""", "PascalCase message")]
    public void ExtractErrorMessage_ValidErrorJson_ReturnsMessage(string json, string expected)
    {
        var result = ResponseParser.ExtractErrorMessage(json);

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("{invalid json")]
    [InlineData("""{"unrelated": "value"}""")]
    public void ExtractErrorMessage_InvalidOrMissingErrorProperty_ReturnsNull(string? content)
    {
        var result = ResponseParser.ExtractErrorMessage(content);

        result.Should().BeNull();
    }
}
