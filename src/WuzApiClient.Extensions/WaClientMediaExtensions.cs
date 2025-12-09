using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WuzApiClient.Common.Models;
using WuzApiClient.Common.Results;
using WuzApiClient.Core.Interfaces;
using WuzApiClient.Extensions.Utilities;
using WuzApiClient.Models.Requests.Chat;
using WuzApiClient.Models.Responses.Chat;

namespace WuzApiClient.Extensions;

/// <summary>
/// Extension methods for IWaClient that provide convenient media sending from files and streams.
/// </summary>
public static class WaClientMediaExtensions
{
    private const long MaxImageSizeBytes = 16_777_216;      // 16 MB
    private const long MaxDocumentSizeBytes = 104_857_600;  // 100 MB

    #region SendImageFromFile

    /// <summary>
    /// Sends an image message from a file.
    /// </summary>
    /// <param name="client">The WaClient instance.</param>
    /// <param name="phone">Recipient phone number.</param>
    /// <param name="filePath">Path to the image file.</param>
    /// <param name="caption">Optional caption.</param>
    /// <param name="quotedId">Optional message ID to reply to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing send response or error.</returns>
    public static async Task<WuzResult<SendMessageResponse>> SendImageFromFileAsync(
        this IWaClient client,
        Phone phone,
        string filePath,
        string? caption = null,
        string? quotedId = null,
        CancellationToken cancellationToken = default)
    {
        if (client is null)
        {
            throw new ArgumentNullException(nameof(client));
        }

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
        if (fileInfo.Length > MaxImageSizeBytes)
        {
            return WuzResult<SendMessageResponse>.Failure(
                WuzApiError.InvalidRequest($"Image file size ({fileInfo.Length} bytes) exceeds maximum allowed size ({MaxImageSizeBytes} bytes)."));
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

        return await client.SendImageAsync(request, cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #region SendImageFromStream

    /// <summary>
    /// Sends an image message from a stream.
    /// </summary>
    /// <param name="client">The WaClient instance.</param>
    /// <param name="phone">Recipient phone number.</param>
    /// <param name="imageStream">
    /// Stream containing the image data. The caller retains ownership and is responsible
    /// for disposing the stream after this method returns.
    /// </param>
    /// <param name="mimeType">
    /// MIME type (e.g., "image/png"). If null, defaults to "image/jpeg".
    /// </param>
    /// <param name="caption">Optional caption.</param>
    /// <param name="quotedId">Optional message ID to reply to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing send response or error.</returns>
    /// <remarks>
    /// This method reads the entire stream content but does NOT dispose it.
    /// Wrap the stream in a using block or dispose it manually after calling this method.
    /// Peak memory usage is approximately 3.5x the file size during encoding.
    /// </remarks>
    public static async Task<WuzResult<SendMessageResponse>> SendImageFromStreamAsync(
        this IWaClient client,
        Phone phone,
        Stream imageStream,
        string? mimeType = null,
        string? caption = null,
        string? quotedId = null,
        CancellationToken cancellationToken = default)
    {
        if (client is null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        // Validate imageStream parameter
        if (imageStream is null)
        {
            return WuzResult<SendMessageResponse>.Failure(
                WuzApiError.InvalidRequest("Image stream cannot be null."));
        }

        // Check stream size if seekable (16MB max for images)
        if (imageStream.CanSeek)
        {
            var streamLength = imageStream.Length;
            if (streamLength > MaxImageSizeBytes)
            {
                return WuzResult<SendMessageResponse>.Failure(
                    WuzApiError.InvalidRequest($"Image stream size ({streamLength} bytes) exceeds maximum allowed size ({MaxImageSizeBytes} bytes)."));
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

        return await client.SendImageAsync(request, cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #region SendDocumentFromFile

    /// <summary>
    /// Sends a document message from a file.
    /// </summary>
    /// <param name="client">The WaClient instance.</param>
    /// <param name="phone">Recipient phone number.</param>
    /// <param name="filePath">Path to the document file.</param>
    /// <param name="caption">Optional caption.</param>
    /// <param name="quotedId">Optional message ID to reply to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing send response or error.</returns>
    public static async Task<WuzResult<SendMessageResponse>> SendDocumentFromFileAsync(
        this IWaClient client,
        Phone phone,
        string filePath,
        string? caption = null,
        string? quotedId = null,
        CancellationToken cancellationToken = default)
    {
        if (client is null)
        {
            throw new ArgumentNullException(nameof(client));
        }

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
        if (fileInfo.Length > MaxDocumentSizeBytes)
        {
            return WuzResult<SendMessageResponse>.Failure(
                WuzApiError.InvalidRequest($"Document file size ({fileInfo.Length} bytes) exceeds maximum allowed size ({MaxDocumentSizeBytes} bytes)."));
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

        return await client.SendDocumentAsync(request, cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #region SendDocumentFromStream

    /// <summary>
    /// Sends a document message from a stream.
    /// </summary>
    /// <param name="client">The WaClient instance.</param>
    /// <param name="phone">Recipient phone number.</param>
    /// <param name="documentStream">
    /// Stream containing the document data. The caller retains ownership and is responsible
    /// for disposing the stream after this method returns.
    /// </param>
    /// <param name="fileName">
    /// File name displayed to recipient in WhatsApp (e.g., "report.pdf").
    /// Used to detect MIME type if mimeType parameter is null.
    /// </param>
    /// <param name="mimeType">MIME type (auto-detected from fileName extension if null).</param>
    /// <param name="caption">Optional caption.</param>
    /// <param name="quotedId">Optional message ID to reply to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing send response or error.</returns>
    /// <remarks>
    /// This method reads the entire stream content but does NOT dispose it.
    /// Wrap the stream in a using block or dispose it manually after calling this method.
    /// Peak memory usage is approximately 3.5x the file size during encoding.
    /// </remarks>
    public static async Task<WuzResult<SendMessageResponse>> SendDocumentFromStreamAsync(
        this IWaClient client,
        Phone phone,
        Stream documentStream,
        string fileName,
        string? mimeType = null,
        string? caption = null,
        string? quotedId = null,
        CancellationToken cancellationToken = default)
    {
        if (client is null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        // Validate documentStream parameter
        if (documentStream is null)
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
        if (documentStream.CanSeek)
        {
            var streamLength = documentStream.Length;
            if (streamLength > MaxDocumentSizeBytes)
            {
                return WuzResult<SendMessageResponse>.Failure(
                    WuzApiError.InvalidRequest($"Document stream size ({streamLength} bytes) exceeds maximum allowed size ({MaxDocumentSizeBytes} bytes)."));
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

        return await client.SendDocumentAsync(request, cancellationToken).ConfigureAwait(false);
    }

    #endregion
}
