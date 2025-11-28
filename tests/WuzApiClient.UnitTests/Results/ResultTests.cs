using AwesomeAssertions;
using WuzApiClient.Results;

namespace WuzApiClient.UnitTests.Results;

[Trait("Category", "Unit")]
public sealed class ResultTests
{
    [Fact]
    public void Success_CreatesSuccessfulResult()
    {
        var result = WuzResult.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
    }

    [Fact]
    public void Failure_CreatesFailedResult()
    {
        var error = new WuzApiError(WuzApiErrorCode.BadRequest, "Test error");

        var result = WuzResult.Failure(error);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Failure_NullError_ThrowsArgumentNullException()
    {
        var act = () => WuzResult.Failure(null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("error");
    }

    [Fact]
    public void Error_OnSuccess_ThrowsInvalidOperationException()
    {
        var result = WuzResult.Success();

        var act = () => result.Error;

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*successful*");
    }

    [Fact]
    public void Match_Success_InvokesOnSuccess()
    {
        var result = WuzResult.Success();
        var wasOnSuccessCalled = false;

        var returnValue = result.Match(
            onSuccess: () =>
            {
                wasOnSuccessCalled = true;
                return "success";
            },
            onFailure: _ => "failure");

        wasOnSuccessCalled.Should().BeTrue();
        returnValue.Should().Be("success");
    }

    [Fact]
    public void Match_Failure_InvokesOnFailure()
    {
        var error = new WuzApiError(WuzApiErrorCode.BadRequest, "Test error");
        var result = WuzResult.Failure(error);
        WuzApiError? capturedError = null;

        var returnValue = result.Match(
            onSuccess: () => "success",
            onFailure: e =>
            {
                capturedError = e;
                return "failure";
            });

        capturedError.Should().Be(error);
        returnValue.Should().Be("failure");
    }

    [Fact]
    public void Match_NullOnSuccess_ThrowsArgumentNullException()
    {
        var result = WuzResult.Success();

        var act = () => result.Match<string>(null!, _ => "failure");

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("onSuccess");
    }

    [Fact]
    public void Match_NullOnFailure_ThrowsArgumentNullException()
    {
        var result = WuzResult.Success();

        var act = () => result.Match(() => "success", (Func<WuzApiError, string>)null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("onFailure");
    }

    [Fact]
    public void ImplicitConversion_FromError_CreatesFailure()
    {
        var error = new WuzApiError(WuzApiErrorCode.BadRequest, "Test error");

        WuzResult result = error;

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void Equals_TwoSuccesses_ReturnsTrue()
    {
        var result1 = WuzResult.Success();
        var result2 = WuzResult.Success();

        result1.Equals(result2).Should().BeTrue();
        (result1 == result2).Should().BeTrue();
    }

    [Fact]
    public void Equals_SuccessAndFailure_ReturnsFalse()
    {
        var success = WuzResult.Success();
        var failure = WuzResult.Failure(new WuzApiError(WuzApiErrorCode.BadRequest, "Test"));

        success.Equals(failure).Should().BeFalse();
        (success == failure).Should().BeFalse();
        (success != failure).Should().BeTrue();
    }

    [Fact]
    public void ToString_Success_ReturnsExpectedFormat()
    {
        var result = WuzResult.Success();

        result.ToString().Should().Be("Success");
    }

    [Fact]
    public void ToString_Failure_IncludesErrorInfo()
    {
        var error = new WuzApiError(WuzApiErrorCode.BadRequest, "Test error");
        var result = WuzResult.Failure(error);

        result.ToString().Should().StartWith("Failure:");
        result.ToString().Should().Contain("Test error");
    }
}
