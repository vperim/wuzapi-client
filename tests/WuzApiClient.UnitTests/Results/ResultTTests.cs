using AwesomeAssertions;
using WuzApiClient.Results;

namespace WuzApiClient.UnitTests.Results;

[Trait("Category", "Unit")]
public sealed class ResultTTests
{
    [Fact]
    public void Success_WithValue_StoresValue()
    {
        var result = WuzResult<int>.Success(42);

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void Success_WithNullValue_AllowsNull()
    {
        var result = WuzResult<string?>.Success(null);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    [Fact]
    public void Failure_CreatesFailedResult()
    {
        var error = new WuzApiError(WuzApiErrorCode.BadRequest, "Test error");

        var result = WuzResult<int>.Failure(error);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void Failure_NullError_ThrowsArgumentNullException()
    {
        var act = () => WuzResult<int>.Failure(null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("error");
    }

    [Fact]
    public void Value_OnFailure_ThrowsInvalidOperationException()
    {
        var error = new WuzApiError(WuzApiErrorCode.BadRequest, "Test error");
        var result = WuzResult<int>.Failure(error);

        var act = () => result.Value;

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*failed*");
    }

    [Fact]
    public void Error_OnSuccess_ThrowsInvalidOperationException()
    {
        var result = WuzResult<int>.Success(42);

        var act = () => result.Error;

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*successful*");
    }

    [Fact]
    public void Match_Success_InvokesOnSuccessWithValue()
    {
        var result = WuzResult<int>.Success(42);
        int? capturedValue = null;

        var returnValue = result.Match(
            onSuccess: v =>
            {
                capturedValue = v;
                return "success";
            },
            onFailure: _ => "failure");

        capturedValue.Should().Be(42);
        returnValue.Should().Be("success");
    }

    [Fact]
    public void Match_Failure_InvokesOnFailureWithError()
    {
        var error = new WuzApiError(WuzApiErrorCode.BadRequest, "Test error");
        var result = WuzResult<int>.Failure(error);
        WuzApiError? capturedError = null;

        var returnValue = result.Match(
            onSuccess: _ => "success",
            onFailure: e =>
            {
                capturedError = e;
                return "failure";
            });

        capturedError.Should().Be(error);
        returnValue.Should().Be("failure");
    }

    [Fact]
    public void GetValueOrDefault_Success_ReturnsValue()
    {
        var result = WuzResult<int>.Success(42);

        var value = result.GetValueOrDefault(0);

        value.Should().Be(42);
    }

    [Fact]
    public void GetValueOrDefault_Failure_ReturnsDefault()
    {
        var error = new WuzApiError(WuzApiErrorCode.BadRequest, "Test error");
        var result = WuzResult<int>.Failure(error);

        var value = result.GetValueOrDefault(99);

        value.Should().Be(99);
    }

    [Fact]
    public void ImplicitConversion_FromValue_CreatesSuccess()
    {
        WuzResult<int> result = 42;

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void ImplicitConversion_FromError_CreatesFailure()
    {
        var error = new WuzApiError(WuzApiErrorCode.BadRequest, "Test error");

        WuzResult<int> result = error;

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void Equals_SameValues_ReturnsTrue()
    {
        var result1 = WuzResult<int>.Success(42);
        var result2 = WuzResult<int>.Success(42);

        result1.Equals(result2).Should().BeTrue();
        (result1 == result2).Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentValues_ReturnsFalse()
    {
        var result1 = WuzResult<int>.Success(42);
        var result2 = WuzResult<int>.Success(99);

        result1.Equals(result2).Should().BeFalse();
        (result1 == result2).Should().BeFalse();
        (result1 != result2).Should().BeTrue();
    }
}
