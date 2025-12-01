using System.Threading;
using System.Threading.Tasks;
using WuzApiClient.Models.Responses.Download;
using WuzApiClient.Results;

namespace WuzApiClient.Core.Implementations;

// Media download methods - to be implemented by Batch 3 Stream A
public sealed partial class WaClient
{
    /// <inheritdoc/>
    public async Task<WuzResult<MediaDownloadResponse>> DownloadImageAsync(
        string messageId,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.GetAsync<MediaDownloadResponse>(
            $"/download/image?id={messageId}",
            "Token",
            this.UserToken,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult<MediaDownloadResponse>> DownloadVideoAsync(
        string messageId,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.GetAsync<MediaDownloadResponse>(
            $"/download/video?id={messageId}",
            "Token",
            this.UserToken,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult<MediaDownloadResponse>> DownloadAudioAsync(
        string messageId,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.GetAsync<MediaDownloadResponse>(
            $"/download/audio?id={messageId}",
            "Token",
            this.UserToken,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult<MediaDownloadResponse>> DownloadDocumentAsync(
        string messageId,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.GetAsync<MediaDownloadResponse>(
            $"/download/document?id={messageId}",
            "Token",
            this.UserToken,
            cancellationToken).ConfigureAwait(false);
    }
}
