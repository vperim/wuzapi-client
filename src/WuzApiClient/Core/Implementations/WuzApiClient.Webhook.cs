using System.Threading;
using System.Threading.Tasks;
using WuzApiClient.Models.Requests.Webhook;
using WuzApiClient.Models.Responses.Webhook;
using WuzApiClient.Results;

namespace WuzApiClient.Core.Implementations;

// Webhook and HMAC configuration methods - to be implemented by Batch 3 Stream B
public sealed partial class WuzApiClient
{
    /// <inheritdoc/>
    public async Task<WuzResult> SetWebhookAsync(
        SetWebhookRequest request,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.PostAsync(
            "/webhook",
            "Token",
            this.UserToken,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult<WebhookConfigResponse>> GetWebhookAsync(
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.GetAsync<WebhookConfigResponse>(
            "/webhook",
            "Token",
            this.UserToken,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult> SetHmacKeyAsync(
        SetHmacKeyRequest request,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.PostAsync(
            "/session/hmac/config",
            "Token",
            this.UserToken,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult<HmacConfigResponse>> GetHmacConfigAsync(
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.GetAsync<HmacConfigResponse>(
            "/session/hmac/config",
            "Token",
            this.UserToken,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult> RemoveHmacConfigAsync(
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.DeleteAsync(
            "/session/hmac/config",
            "Token",
            this.UserToken,
            cancellationToken).ConfigureAwait(false);
    }
}
