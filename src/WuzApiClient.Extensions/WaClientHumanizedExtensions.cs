using System;
using System.Threading;
using System.Threading.Tasks;
using WuzApiClient.Common.Models;
using WuzApiClient.Common.Results;
using WuzApiClient.Core.Interfaces;
using WuzApiClient.Extensions.Options;
using WuzApiClient.Models.Requests.Chat;
using WuzApiClient.Models.Responses.Chat;

namespace WuzApiClient.Extensions;

/// <summary>
/// Extension methods for IWaClient that provide humanized message sending with typing simulation.
/// </summary>
public static class WaClientHumanizedExtensions
{
    private static readonly ThreadLocal<Random> Rng = new(() => new Random());

    /// <summary>
    /// Sends a text message with simulated typing behavior.
    /// Shows a typing indicator and delays sending based on message length.
    /// </summary>
    /// <param name="client">The WaClient instance.</param>
    /// <param name="phone">The recipient phone number.</param>
    /// <param name="message">The message text.</param>
    /// <param name="options">Options controlling the typing simulation. Uses <see cref="HumanizedTypingOptions.Default"/> if null.</param>
    /// <param name="quotedId">Optional message ID to reply to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing send response or error.</returns>
    public static async Task<WuzResult<SendMessageResponse>> SendTextMessageHumanizedAsync(
        this IWaClient client,
        Phone phone,
        string message,
        HumanizedTypingOptions? options = null,
        string? quotedId = null,
        CancellationToken cancellationToken = default)
    {
        if (client == null) throw new ArgumentNullException(nameof(client));
        if (message == null) throw new ArgumentNullException(nameof(message));

        options ??= HumanizedTypingOptions.Default;
        var delay = CalculateDelay(message.Length, options);

        if (options.ShowTypingIndicator && delay > TimeSpan.Zero)
        {
            // Fire-and-forget: presence indicator is non-critical
            // We intentionally don't await this to avoid adding latency
            _ = client.SetPresenceAsync(
                new SetPresenceRequest
                {
                    Phone = phone,
                    State = "composing",
                    Media = string.Empty
                },
                cancellationToken);
        }

        if (delay > TimeSpan.Zero)
        {
            try
            {
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // Clear typing indicator on cancellation
                _ = client.SetPresenceAsync(
                    new SetPresenceRequest
                    {
                        Phone = phone,
                        State = "paused",
                        Media = string.Empty
                    },
                    CancellationToken.None);
                throw;
            }
        }

        return await client.SendTextMessageAsync(phone, message, quotedId, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Calculates the delay based on message length and options.
    /// Formula: min(BaseDelay + (charCount * PerCharacterDelay), MaxDelay) + random(-Jitter, +Jitter)
    /// </summary>
    internal static TimeSpan CalculateDelay(int characterCount, HumanizedTypingOptions options)
    {
        var rawMs = options.BaseDelay.TotalMilliseconds
                  + (characterCount * options.PerCharacterDelay.TotalMilliseconds);

        var cappedMs = Math.Min(rawMs, options.MaxDelay.TotalMilliseconds);

        // Generate random value between -1 and +1, then scale by jitter
        // ThreadLocal<Random> ensures thread safety without locking
        var jitter = (Rng.Value!.NextDouble() * 2 - 1) * options.Jitter.TotalMilliseconds;

        var finalMs = Math.Max(0, cappedMs + jitter);
        return TimeSpan.FromMilliseconds(finalMs);
    }
}
