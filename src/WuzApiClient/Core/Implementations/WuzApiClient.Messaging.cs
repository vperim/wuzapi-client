using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WuzApiClient.Models.Common;
using WuzApiClient.Models.Requests.Chat;
using WuzApiClient.Models.Responses.Chat;
using WuzApiClient.Results;
using WuzApiClient.Utilities;

namespace WuzApiClient.Core.Implementations;

// Messaging methods - to be implemented by Batch 1 Stream B
public sealed partial class WuzApiClient
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

    /// <inheritdoc/>
    public async Task<WuzResult<SendMessageResponse>> SendImageFromFileAsync(
        Phone phone,
        string filePath,
        string? caption = null,
        string? quotedId = null,
        CancellationToken cancellationToken = default)
    {
        // Validate filePath parameter
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return WuzResult<SendMessageResponse>.Failure(
                WuzApiError.InvalidRequest("File path cannot be null or empty."));
        }

        // Check if file exists
        if (!File.Exists(filePath))
        {
            return WuzResult<SendMessageResponse>.Failure(
                WuzApiError.InvalidRequest($"File not found: {filePath}"));
        }

        // Check file size (16MB max for images)
        var fileInfo = new FileInfo(filePath);
        const long maxImageSize = 16_777_216; // 16 MB
        if (fileInfo.Length > maxImageSize)
        {
            return WuzResult<SendMessageResponse>.Failure(
                WuzApiError.InvalidRequest($"Image file size ({fileInfo.Length} bytes) exceeds maximum allowed size ({maxImageSize} bytes)."));
        }

        // Detect MIME type from file extension
        var mimeType = MimeTypeDetector.DetectFromExtension(filePath);

        // Encode file to base64 data URL
        var encodeResult = await FileEncoder.EncodeFileAsync(
            filePath,
            mimeType,
            cancellationToken).ConfigureAwait(false);

        if (!encodeResult.IsSuccess)
        {
            return WuzResult<SendMessageResponse>.Failure(
                WuzApiError.InvalidRequest($"Failed to encode image file: {encodeResult.Error}"));
        }

        // Create request and delegate to existing SendImageAsync
        var request = new SendImageRequest
        {
            Phone = phone,
            Image = encodeResult.EncodedData,
            Caption = caption,
            MimeType = mimeType,
            QuotedId = quotedId
        };

        return await SendImageAsync(request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult<SendMessageResponse>> SendImageFromStreamAsync(
        Phone phone,
        Stream imageStream,
        string? mimeType = null,
        string? caption = null,
        string? quotedId = null,
        CancellationToken cancellationToken = default)
    {
        // Validate imageStream parameter
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (imageStream == null)
        {
            return WuzResult<SendMessageResponse>.Failure(
                WuzApiError.InvalidRequest("Image stream cannot be null."));
        }

        // Check stream size if seekable (16MB max for images)
        const long maxImageSize = 16_777_216; // 16 MB
        if (imageStream.CanSeek)
        {
            var streamLength = imageStream.Length;
            if (streamLength > maxImageSize)
            {
                return WuzResult<SendMessageResponse>.Failure(
                    WuzApiError.InvalidRequest($"Image stream size ({streamLength} bytes) exceeds maximum allowed size ({maxImageSize} bytes)."));
            }
        }

        // Default to "image/jpeg" if mimeType is null
        var effectiveMimeType = mimeType ?? "image/jpeg";

        // Encode stream to base64 data URL
        var encodedImage = await FileEncoder.EncodeStreamAsync(
            imageStream,
            effectiveMimeType,
            cancellationToken).ConfigureAwait(false);

        // Create request and delegate to existing SendImageAsync
        var request = new SendImageRequest
        {
            Phone = phone,
            Image = encodedImage,
            Caption = caption,
            MimeType = effectiveMimeType,
            QuotedId = quotedId
        };

        return await SendImageAsync(request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult<SendMessageResponse>> SendDocumentFromFileAsync(
        Phone phone,
        string filePath,
        string? caption = null,
        string? quotedId = null,
        CancellationToken cancellationToken = default)
    {
        // Validate filePath parameter
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return WuzResult<SendMessageResponse>.Failure(
                WuzApiError.InvalidRequest("File path cannot be null or empty."));
        }

        // Check if file exists
        if (!File.Exists(filePath))
        {
            return WuzResult<SendMessageResponse>.Failure(
                WuzApiError.InvalidRequest($"File not found: {filePath}"));
        }

        // Check file size (100MB max for documents)
        var fileInfo = new FileInfo(filePath);
        const long maxDocumentSize = 104_857_600; // 100 MB
        if (fileInfo.Length > maxDocumentSize)
        {
            return WuzResult<SendMessageResponse>.Failure(
                WuzApiError.InvalidRequest($"Document file size ({fileInfo.Length} bytes) exceeds maximum allowed size ({maxDocumentSize} bytes)."));
        }

        // Extract file name from path
        var fileName = Path.GetFileName(filePath);

        // Detect MIME type from file extension
        var mimeType = MimeTypeDetector.DetectFromExtension(filePath);

        // Encode file to base64 data URL with application/octet-stream (required by WuzAPI)
        var encodeResult = await FileEncoder.EncodeFileAsync(
            filePath,
            "application/octet-stream",
            cancellationToken).ConfigureAwait(false);

        if (!encodeResult.IsSuccess)
        {
            return WuzResult<SendMessageResponse>.Failure(
                WuzApiError.InvalidRequest($"Failed to encode document file: {encodeResult.Error}"));
        }

        // Create request and delegate to existing SendDocumentAsync
        var request = new SendDocumentRequest
        {
            Phone = phone,
            Document = encodeResult.EncodedData,
            FileName = fileName,
            MimeType = mimeType,
            Caption = caption,
            QuotedId = quotedId
        };

        return await SendDocumentAsync(request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult<SendMessageResponse>> SendDocumentFromStreamAsync(
        Phone phone,
        Stream documentStream,
        string fileName,
        string? mimeType = null,
        string? caption = null,
        string? quotedId = null,
        CancellationToken cancellationToken = default)
    {
        // Validate documentStream parameter
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (documentStream == null)
        {
            return WuzResult<SendMessageResponse>.Failure(
                WuzApiError.InvalidRequest("Document stream cannot be null."));
        }

        // Validate fileName parameter
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return WuzResult<SendMessageResponse>.Failure(
                WuzApiError.InvalidRequest("File name cannot be null or empty."));
        }

        // Check stream size if seekable (100MB max for documents)
        const long maxDocumentSize = 104_857_600; // 100 MB
        if (documentStream.CanSeek)
        {
            var streamLength = documentStream.Length;
            if (streamLength > maxDocumentSize)
            {
                return WuzResult<SendMessageResponse>.Failure(
                    WuzApiError.InvalidRequest($"Document stream size ({streamLength} bytes) exceeds maximum allowed size ({maxDocumentSize} bytes)."));
            }
        }

        // Detect MIME type from fileName extension if mimeType is null
        var effectiveMimeType = mimeType ?? MimeTypeDetector.DetectFromExtension(fileName);

        // Encode stream to base64 data URL with application/octet-stream (required by WuzAPI)
        var encodedDocument = await FileEncoder.EncodeStreamAsync(
            documentStream,
            "application/octet-stream",
            cancellationToken).ConfigureAwait(false);

        // Create request and delegate to existing SendDocumentAsync
        var request = new SendDocumentRequest
        {
            Phone = phone,
            Document = encodedDocument,
            FileName = fileName,
            MimeType = effectiveMimeType,
            Caption = caption,
            QuotedId = quotedId
        };

        return await SendDocumentAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
