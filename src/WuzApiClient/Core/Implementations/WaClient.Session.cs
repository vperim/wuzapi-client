using System.Threading;
using System.Threading.Tasks;
using WuzApiClient.Common.Results;
using WuzApiClient.Models.Requests.Session;
using WuzApiClient.Models.Responses.Session;

namespace WuzApiClient.Core.Implementations;

// Session management methods - to be implemented by Batch 1 Stream A
public sealed partial class WaClient
{
    /// <inheritdoc/>
    public async Task<WuzResult> ConnectSessionAsync(
        ConnectSessionRequest request,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.PostAsync(
            "/session/connect",
            "Token",
            this.UserToken,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult> DisconnectSessionAsync(CancellationToken cancellationToken = default)
    {
        return await this.httpClient.PostAsync(
            "/session/disconnect",
            "Token",
            this.UserToken,
            null,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult> LogoutSessionAsync(CancellationToken cancellationToken = default)
    {
        return await this.httpClient.PostAsync(
            "/session/logout",
            "Token",
            this.UserToken,
            null,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult<SessionStatusResponse>> GetSessionStatusAsync(
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.GetAsync<SessionStatusResponse>(
            "/session/status",
            "Token",
            this.UserToken,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult<QrCodeResponse>> GetQrCodeAsync(CancellationToken cancellationToken = default)
    {
        return await this.httpClient.GetAsync<QrCodeResponse>(
            "/session/qr",
            "Token",
            this.UserToken,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult<PairPhoneResponse>> PairPhoneAsync(
        PairPhoneRequest request,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.PostAsync<PairPhoneResponse>(
            "/session/pairphone",
            "Token",
            this.UserToken,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult> SetProxyAsync(
        SetProxyRequest request,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.PostAsync(
            "/session/proxy",
            "Token",
            this.UserToken,
            request,
            cancellationToken).ConfigureAwait(false);
    }
}
