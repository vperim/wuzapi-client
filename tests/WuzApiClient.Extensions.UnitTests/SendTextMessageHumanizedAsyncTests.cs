using AwesomeAssertions;
using NSubstitute;
using WuzApiClient.Common.Models;
using WuzApiClient.Common.Results;
using WuzApiClient.Core.Interfaces;
using WuzApiClient.Extensions.Options;
using WuzApiClient.Models.Requests.Chat;
using WuzApiClient.Models.Responses.Chat;

namespace WuzApiClient.Extensions.UnitTests;

[Trait("Category", "Unit")]
public sealed class SendTextMessageHumanizedAsyncTests
{
    private static readonly Phone TestPhone = Phone.Create("5511999999999");

    [Fact]
    public async Task SendTextMessageHumanizedAsync_NullClient_ThrowsArgumentNullException()
    {
        // Arrange
        IWaClient client = null!;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            client.SendTextMessageHumanizedAsync(TestPhone, "Hello"));
    }

    [Fact]
    public async Task SendTextMessageHumanizedAsync_NullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        var client = Substitute.For<IWaClient>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            client.SendTextMessageHumanizedAsync(TestPhone, null!));
    }

    [Fact]
    public async Task SendTextMessageHumanizedAsync_CallsSendTextMessageAsync()
    {
        // Arrange
        var client = Substitute.For<IWaClient>();
        var expectedResponse = WuzResult<SendMessageResponse>.Success(
            new SendMessageResponse { Id = "msg-123", Timestamp = 1700000000 });
        client.SendTextMessageAsync(TestPhone, "Hello", null, Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        var options = new HumanizedTypingOptions
        {
            BaseDelay = TimeSpan.Zero,
            PerCharacterDelay = TimeSpan.Zero,
            MaxDelay = TimeSpan.Zero,
            Jitter = TimeSpan.Zero,
            ShowTypingIndicator = false
        };

        // Act
        var result = await client.SendTextMessageHumanizedAsync(TestPhone, "Hello", options);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be("msg-123");
        await client.Received(1).SendTextMessageAsync(TestPhone, "Hello", null, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendTextMessageHumanizedAsync_PassesQuotedId()
    {
        // Arrange
        var client = Substitute.For<IWaClient>();
        var expectedResponse = WuzResult<SendMessageResponse>.Success(
            new SendMessageResponse { Id = "msg-123" });
        client.SendTextMessageAsync(TestPhone, "Hello", "quote-id", Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        var options = new HumanizedTypingOptions
        {
            BaseDelay = TimeSpan.Zero,
            PerCharacterDelay = TimeSpan.Zero,
            MaxDelay = TimeSpan.Zero,
            Jitter = TimeSpan.Zero,
            ShowTypingIndicator = false
        };

        // Act
        await client.SendTextMessageHumanizedAsync(TestPhone, "Hello", options, quotedId: "quote-id");

        // Assert
        await client.Received(1).SendTextMessageAsync(TestPhone, "Hello", "quote-id", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendTextMessageHumanizedAsync_ShowTypingIndicatorTrue_CallsSetPresenceAsync()
    {
        // Arrange
        var client = Substitute.For<IWaClient>();
        client.SendTextMessageAsync(Arg.Any<Phone>(), Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(WuzResult<SendMessageResponse>.Success(new SendMessageResponse()));
        client.SetPresenceAsync(Arg.Any<SetPresenceRequest>(), Arg.Any<CancellationToken>())
            .Returns(WuzResult.Success());

        var options = new HumanizedTypingOptions
        {
            BaseDelay = TimeSpan.FromMilliseconds(10),
            PerCharacterDelay = TimeSpan.Zero,
            MaxDelay = TimeSpan.FromSeconds(1),
            Jitter = TimeSpan.Zero,
            ShowTypingIndicator = true
        };

        // Act
        await client.SendTextMessageHumanizedAsync(TestPhone, "Hello", options);

        // Assert
        await client.Received(1).SetPresenceAsync(
            Arg.Is<SetPresenceRequest>(r => r.Phone == TestPhone && r.State == "composing"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendTextMessageHumanizedAsync_ShowTypingIndicatorFalse_DoesNotCallSetPresenceAsync()
    {
        // Arrange
        var client = Substitute.For<IWaClient>();
        client.SendTextMessageAsync(Arg.Any<Phone>(), Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(WuzResult<SendMessageResponse>.Success(new SendMessageResponse()));

        var options = new HumanizedTypingOptions
        {
            BaseDelay = TimeSpan.FromMilliseconds(10),
            PerCharacterDelay = TimeSpan.Zero,
            MaxDelay = TimeSpan.FromSeconds(1),
            Jitter = TimeSpan.Zero,
            ShowTypingIndicator = false
        };

        // Act
        await client.SendTextMessageHumanizedAsync(TestPhone, "Hello", options);

        // Assert
        await client.DidNotReceive().SetPresenceAsync(Arg.Any<SetPresenceRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendTextMessageHumanizedAsync_ZeroDelay_DoesNotShowTypingIndicator()
    {
        // Arrange
        var client = Substitute.For<IWaClient>();
        client.SendTextMessageAsync(Arg.Any<Phone>(), Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(WuzResult<SendMessageResponse>.Success(new SendMessageResponse()));

        var options = new HumanizedTypingOptions
        {
            BaseDelay = TimeSpan.Zero,
            PerCharacterDelay = TimeSpan.Zero,
            MaxDelay = TimeSpan.Zero,
            Jitter = TimeSpan.Zero,
            ShowTypingIndicator = true // Even though true, zero delay means no indicator
        };

        // Act
        await client.SendTextMessageHumanizedAsync(TestPhone, "Hello", options);

        // Assert - should not call SetPresenceAsync when delay is zero
        await client.DidNotReceive().SetPresenceAsync(Arg.Any<SetPresenceRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendTextMessageHumanizedAsync_UsesDefaultOptionsWhenNull()
    {
        // Arrange
        var client = Substitute.For<IWaClient>();
        client.SendTextMessageAsync(Arg.Any<Phone>(), Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(WuzResult<SendMessageResponse>.Success(new SendMessageResponse { Id = "msg-1" }));
        client.SetPresenceAsync(Arg.Any<SetPresenceRequest>(), Arg.Any<CancellationToken>())
            .Returns(WuzResult.Success());

        // Act
        var result = await client.SendTextMessageHumanizedAsync(TestPhone, "Hello", options: null);

        // Assert - should use default options which have ShowTypingIndicator = true
        result.IsSuccess.Should().BeTrue();
        await client.Received(1).SetPresenceAsync(
            Arg.Is<SetPresenceRequest>(r => r.State == "composing"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendTextMessageHumanizedAsync_Cancellation_ClearsTypingIndicator()
    {
        // Arrange
        var client = Substitute.For<IWaClient>();
        var cts = new CancellationTokenSource();
        client.SetPresenceAsync(Arg.Any<SetPresenceRequest>(), Arg.Any<CancellationToken>())
            .Returns(WuzResult.Success());

        var options = new HumanizedTypingOptions
        {
            BaseDelay = TimeSpan.FromSeconds(5), // Long delay to allow cancellation
            PerCharacterDelay = TimeSpan.Zero,
            MaxDelay = TimeSpan.FromSeconds(10),
            Jitter = TimeSpan.Zero,
            ShowTypingIndicator = true
        };

        // Act
        cts.CancelAfter(TimeSpan.FromMilliseconds(50));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            client.SendTextMessageHumanizedAsync(TestPhone, "Hello", options, cancellationToken: cts.Token));

        // Allow time for fire-and-forget call to complete
        await Task.Delay(100);

        // Assert - should have called SetPresenceAsync with "paused" to clear indicator
        await client.Received().SetPresenceAsync(
            Arg.Is<SetPresenceRequest>(r => r.State == "paused"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendTextMessageHumanizedAsync_EmptyMessage_UsesBaseDelayOnly()
    {
        // Arrange
        var client = Substitute.For<IWaClient>();
        client.SendTextMessageAsync(Arg.Any<Phone>(), Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(WuzResult<SendMessageResponse>.Success(new SendMessageResponse { Id = "msg-123" }));

        var options = new HumanizedTypingOptions
        {
            BaseDelay = TimeSpan.Zero,
            PerCharacterDelay = TimeSpan.FromMilliseconds(50), // Would add delay if chars > 0
            MaxDelay = TimeSpan.FromSeconds(8),
            Jitter = TimeSpan.Zero,
            ShowTypingIndicator = false
        };

        // Act
        var result = await client.SendTextMessageHumanizedAsync(TestPhone, string.Empty, options);

        // Assert - should succeed with zero delay (no per-character contribution)
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be("msg-123");
        await client.Received(1).SendTextMessageAsync(TestPhone, string.Empty, null, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendTextMessageHumanizedAsync_PropagatesClientError()
    {
        // Arrange
        var client = Substitute.For<IWaClient>();
        var error = new WuzApiError(WuzApiErrorCode.BadRequest, "Invalid phone");
        client.SendTextMessageAsync(Arg.Any<Phone>(), Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(WuzResult<SendMessageResponse>.Failure(error));

        var options = new HumanizedTypingOptions
        {
            BaseDelay = TimeSpan.Zero,
            PerCharacterDelay = TimeSpan.Zero,
            MaxDelay = TimeSpan.Zero,
            Jitter = TimeSpan.Zero,
            ShowTypingIndicator = false
        };

        // Act
        var result = await client.SendTextMessageHumanizedAsync(TestPhone, "Hello", options);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(WuzApiErrorCode.BadRequest);
        result.Error.Message.Should().Be("Invalid phone");
    }
}
