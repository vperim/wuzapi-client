using System.Threading;
using System.Threading.Tasks;
using WuzApiClient.Common.Results;
using WuzApiClient.Models.Requests.Chat;

namespace WuzApiClient.Core.Implementations;

// Chat operations methods - to be implemented by Batch 1 Stream C
public sealed partial class WaClient
{
    /// <inheritdoc/>
    public async Task<WuzResult> MarkAsReadAsync(
        MarkAsReadRequest request,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.PostAsync(
            "/chat/markread",
            "Token",
            this.UserToken,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult> ReactToMessageAsync(
        ReactRequest request,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.PostAsync(
            "/chat/react",
            "Token",
            this.UserToken,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult> SetPresenceAsync(
        SetPresenceRequest request,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.PostAsync(
            "/chat/presence",
            "Token",
            this.UserToken,
            request,
            cancellationToken).ConfigureAwait(false);
    }
}
