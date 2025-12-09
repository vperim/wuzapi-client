using System.Threading;
using System.Threading.Tasks;
using WuzApiClient.Common.Results;
using WuzApiClient.Common.Models;
using WuzApiClient.Models.Requests.Chat;
using WuzApiClient.Models.Responses.Chat;

namespace WuzApiClient.Core.Implementations;

// Messaging methods - to be implemented by Batch 1 Stream B
public sealed partial class WaClient
{
    /// <inheritdoc/>
    public async Task<WuzResult<SendMessageResponse>> SendTextMessageAsync(
        Phone phone,
        string message,
        string? quotedId = null,
        CancellationToken cancellationToken = default)
    {
        var request = new SendTextMessageRequest
        {
            Phone = phone,
            Body = message,
            QuotedId = quotedId
        };

        return await this.httpClient.PostAsync<SendMessageResponse>(
            "/chat/send/text",
            "Token",
            this.UserToken,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult<SendMessageResponse>> SendImageAsync(
        SendImageRequest request,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.PostAsync<SendMessageResponse>(
            "/chat/send/image",
            "Token",
            this.UserToken,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult<SendMessageResponse>> SendDocumentAsync(
        SendDocumentRequest request,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.PostAsync<SendMessageResponse>(
            "/chat/send/document",
            "Token",
            this.UserToken,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult<SendMessageResponse>> SendAudioAsync(
        SendAudioRequest request,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.PostAsync<SendMessageResponse>(
            "/chat/send/audio",
            "Token",
            this.UserToken,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult<SendMessageResponse>> SendVideoAsync(
        SendVideoRequest request,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.PostAsync<SendMessageResponse>(
            "/chat/send/video",
            "Token",
            this.UserToken,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult<SendMessageResponse>> SendStickerAsync(
        SendStickerRequest request,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.PostAsync<SendMessageResponse>(
            "/chat/send/sticker",
            "Token",
            this.UserToken,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult<SendMessageResponse>> SendTemplateAsync(
        SendTemplateRequest request,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.PostAsync<SendMessageResponse>(
            "/chat/send/template",
            "Token",
            this.UserToken,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult<SendMessageResponse>> SendLocationAsync(
        SendLocationRequest request,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.PostAsync<SendMessageResponse>(
            "/chat/send/location",
            "Token",
            this.UserToken,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult<SendMessageResponse>> SendContactAsync(
        SendContactRequest request,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.PostAsync<SendMessageResponse>(
            "/chat/send/contact",
            "Token",
            this.UserToken,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult<SendMessageResponse>> EditMessageAsync(
        EditMessageRequest request,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.PostAsync<SendMessageResponse>(
            "/chat/send/edit",
            "Token",
            this.UserToken,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult> DeleteMessageAsync(
        DeleteMessageRequest request,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.PostAsync(
            "/chat/delete",
            "Token",
            this.UserToken,
            request,
            cancellationToken).ConfigureAwait(false);
    }
}
