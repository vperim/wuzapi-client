using AwesomeAssertions;
using WuzApiClient.Common.Results;

namespace WuzApiClient.UnitTests.Results;

/// <summary>
/// Unit tests for <see cref="WuzApiError"/>.
/// </summary>
[Trait("Category", "Unit")]
public sealed class WuzApiErrorTests
{
    [Fact]
    public void Constructor_ValidCodeAndMessage_SetsProperties()
    {
        // Arrange
        var code = WuzApiErrorCode.BadRequest;
        var message = "Invalid input";

        // Act
        var error = new WuzApiError(code, message);

        // Assert
        error.Code.Should().Be(code);
        error.Message.Should().Be(message);
    }

    [Fact]
    public void Constructor_NullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        var code = WuzApiErrorCode.BadRequest;

        // Act
        var act = () => new WuzApiError(code, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("message");
    }

    [Theory]
    [InlineData(400, WuzApiErrorCode.BadRequest)]
    [InlineData(401, WuzApiErrorCode.Unauthorized)]
    [InlineData(403, WuzApiErrorCode.Forbidden)]
    [InlineData(404, WuzApiErrorCode.NotFound)]
    [InlineData(409, WuzApiErrorCode.Conflict)]
    [InlineData(429, WuzApiErrorCode.RateLimitExceeded)]
    [InlineData(500, WuzApiErrorCode.InternalServerError)]
    [InlineData(503, WuzApiErrorCode.InternalServerError)]
    [InlineData(502, WuzApiErrorCode.InternalServerError)]
    [InlineData(418, WuzApiErrorCode.UnexpectedResponse)]
    [InlineData(200, WuzApiErrorCode.UnexpectedResponse)]
    public void FromHttpStatus_VariousStatusCodes_ReturnsExpectedErrorCode(
        int statusCode,
        WuzApiErrorCode expectedCode)
    {
        // Act
        var error = WuzApiError.FromHttpStatus(statusCode, string.Empty);

        // Assert
        error.Code.Should().Be(expectedCode);
        error.HttpStatusCode.Should().Be(statusCode);
    }

    [Fact]
    public void FromHttpStatus_WithJsonErrorProperty_ExtractsMessage()
    {
        // Arrange
        var responseBody = """{"error": "Something went wrong"}""";

        // Act
        var error = WuzApiError.FromHttpStatus(400, responseBody);

        // Assert
        error.Message.Should().Be("Something went wrong");
        error.ResponseBody.Should().Be(responseBody);
    }

    [Fact]
    public void FromHttpStatus_WithJsonMessageProperty_ExtractsMessage()
    {
        // Arrange
        var responseBody = """{"message": "Resource not found"}""";

        // Act
        var error = WuzApiError.FromHttpStatus(404, responseBody);

        // Assert
        error.Message.Should().Be("Resource not found");
    }

    [Theory]
    [InlineData("not valid json")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void FromHttpStatus_InvalidOrEmptyJson_UsesHttpStatusAsMessage(string? responseBody)
    {
        // Act
        var error = WuzApiError.FromHttpStatus(500, responseBody!);

        // Assert
        error.Message.Should().Be("HTTP 500");
    }

    [Fact]
    public void NetworkError_CreatesCorrectError()
    {
        // Arrange
        var message = "Connection refused";

        // Act
        var error = WuzApiError.NetworkError(message);

        // Assert
        error.Code.Should().Be(WuzApiErrorCode.NetworkError);
        error.Message.Should().Be(message);
    }

    [Fact]
    public void Timeout_CreatesCorrectError()
    {
        // Act
        var error = WuzApiError.Timeout();

        // Assert
        error.Code.Should().Be(WuzApiErrorCode.Timeout);
        error.Message.Should().Be("Request timed out");
    }

    [Fact]
    public void DeserializationError_CreatesCorrectError()
    {
        // Arrange
        var message = "Failed to parse response";

        // Act
        var error = WuzApiError.DeserializationError(message);

        // Assert
        error.Code.Should().Be(WuzApiErrorCode.DeserializationError);
        error.Message.Should().Be(message);
    }

    [Fact]
    public void Equals_SameCodeAndMessage_ReturnsTrue()
    {
        // Arrange
        var error1 = new WuzApiError(WuzApiErrorCode.BadRequest, "Invalid input");
        var error2 = new WuzApiError(WuzApiErrorCode.BadRequest, "Invalid input");

        // Act & Assert
        error1.Equals(error2).Should().BeTrue();
        error1.GetHashCode().Should().Be(error2.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentCode_ReturnsFalse()
    {
        // Arrange
        var error1 = new WuzApiError(WuzApiErrorCode.BadRequest, "Error");
        var error2 = new WuzApiError(WuzApiErrorCode.NotFound, "Error");

        // Act & Assert
        error1.Equals(error2).Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentMessage_ReturnsFalse()
    {
        // Arrange
        var error1 = new WuzApiError(WuzApiErrorCode.BadRequest, "Error 1");
        var error2 = new WuzApiError(WuzApiErrorCode.BadRequest, "Error 2");

        // Act & Assert
        error1.Equals(error2).Should().BeFalse();
    }

    [Fact]
    public void ToString_ReturnsFormattedString()
    {
        // Arrange
        var error = new WuzApiError(WuzApiErrorCode.NotFound, "Resource not found");

        // Act
        var result = error.ToString();

        // Assert
        result.Should().Be("[NotFound] Resource not found");
    }
}
