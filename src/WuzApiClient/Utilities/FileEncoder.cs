using System;
using System.Buffers;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WuzApiClient.Utilities;

/// <summary>
/// Internal utility for encoding files and streams to base64 data URLs.
/// Uses chunked streaming to avoid loading entire files into memory.
/// </summary>
internal static class FileEncoder
{
    private const long MaxImageSizeBytes = 16_777_216;      // 16 MB
    private const long MaxDocumentSizeBytes = 104_857_600;  // 100 MB
    private const long MaxAudioSizeBytes = 16_777_216;      // 16 MB
    private const long MaxVideoSizeBytes = 16_777_216;      // 16 MB

    private const int StreamBufferSize = 81_920; // same as default Stream.CopyToAsync

    /// <summary>
    /// Returns the maximum allowed size in bytes for a given MIME type.
    /// </summary>
    private static long GetMaxAllowedSize(string mimeType)
    {
        if (mimeType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
        {
            return MaxImageSizeBytes;
        }

        if (mimeType.StartsWith("video/", StringComparison.OrdinalIgnoreCase))
        {
            return MaxVideoSizeBytes;
        }

        if (mimeType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase))
        {
            return MaxAudioSizeBytes;
        }

        // Documents and everything else
        return MaxDocumentSizeBytes;
    }

    /// <summary>
    /// Encodes a file to a base64 data URL.
    /// </summary>
    /// <param name="filePath">Path to the file.</param>
    /// <param name="mimeType">MIME type of the file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing encoded data URL or error message.</returns>
    public static async Task<FileEncodeResult> EncodeFileAsync(
        string filePath,
        string mimeType,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var stream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                StreamBufferSize,
                FileOptions.Asynchronous | FileOptions.SequentialScan);

            var length = stream.CanSeek ? stream.Length : (long?)null;
            var maxAllowed = GetMaxAllowedSize(mimeType);

            // Size validation for seekable streams
            if (length > maxAllowed)
            {
                return FileEncodeResult.Failure(
                    $"File size ({length.Value} bytes) exceeds maximum allowed size for {mimeType} ({maxAllowed} bytes).");
            }

            var encodedData = await EncodeStreamInternalAsync(
                stream,
                mimeType,
                length,
                maxAllowed,
                cancellationToken).ConfigureAwait(false);

            return FileEncodeResult.Success(encodedData);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (FileNotFoundException ex)
        {
            return FileEncodeResult.Failure($"File not found: {ex.Message}");
        }
        catch (DirectoryNotFoundException ex)
        {
            return FileEncodeResult.Failure($"Directory not found: {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            return FileEncodeResult.Failure($"Access denied: {ex.Message}");
        }
        catch (IOException ex)
        {
            return FileEncodeResult.Failure($"I/O error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return FileEncodeResult.Failure($"Encoding failed: {ex.GetType().Name} - {ex.Message}");
        }
    }

    /// <summary>
    /// Encodes a stream to a base64 data URL.
    /// For seekable streams: encodes from position 0, then restores the original position.
    /// For non-seekable streams: encodes from the current position to EOF.
    /// </summary>
    /// <param name="stream">Stream containing file data.</param>
    /// <param name="mimeType">MIME type.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Base64 data URL.</returns>
    /// <exception cref="ArgumentException">Thrown when stream size exceeds maximum allowed size for the MIME type.</exception>
    public static async Task<string> EncodeStreamAsync(
        Stream stream,
        string mimeType,
        CancellationToken cancellationToken = default)
    {
        if (stream is null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        if (mimeType is null)
        {
            throw new ArgumentNullException(nameof(mimeType));
        }

        cancellationToken.ThrowIfCancellationRequested();

        var maxAllowedSize = GetMaxAllowedSize(mimeType);

        if (stream.CanSeek)
        {
            var originalPosition = stream.Position;
            var length = stream.Length;

            // Pre-validate size for seekable streams
            if (length > maxAllowedSize)
            {
                throw new ArgumentException(
                    $"Stream size ({length} bytes) exceeds maximum allowed size for {mimeType} ({maxAllowedSize} bytes).",
                    nameof(stream));
            }

            stream.Position = 0;

            try
            {
                return await EncodeStreamInternalAsync(
                    stream,
                    mimeType,
                    knownLength: length,
                    maxAllowedSize: maxAllowedSize,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                stream.Position = originalPosition;
            }
        }

        // Non-seekable: pass maxAllowedSize for running-total check in loop
        return await EncodeStreamInternalAsync(
            stream,
            mimeType,
            knownLength: null,
            maxAllowedSize: maxAllowedSize,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Core chunked base64 encoder that never loads the full stream into memory.
    /// Reads from current stream position to EOF.
    /// </summary>
    private static async Task<string> EncodeStreamInternalAsync(
        Stream stream,
        string mimeType,
        long? knownLength,
        long? maxAllowedSize,
        CancellationToken cancellationToken)
    {
        var prefix = $"data:{mimeType};base64,";

        // Estimate base64 size if we know the length:
        // base64Len = ceil(len / 3) * 4
        var estimatedBase64Len = 0;
        if (knownLength is <= int.MaxValue)
        {
            var len = knownLength.Value;
            estimatedBase64Len = (int)(((len + 2L) / 3L) * 4L);
        }

        var sb = estimatedBase64Len > 0
            ? new StringBuilder(prefix.Length + estimatedBase64Len)
            : new StringBuilder(prefix.Length + 1024); // small default

        sb.Append(prefix);

        var buffer = ArrayPool<byte>.Shared.Rent(StreamBufferSize);
        // Max chars needed for any N bytes is ((N + 2) / 3) * 4
        var charBuffer = ArrayPool<char>.Shared.Rent(((StreamBufferSize + 2) / 3) * 4);

        var leftoverCount = 0;
        var totalBytesRead = 0L;

        try
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Read after any leftover bytes, limiting to StreamBufferSize
                // (buffer.Length may be larger due to ArrayPool)
                var maxBytesToRead = StreamBufferSize - leftoverCount;
                var bytesRead = await stream
                    .ReadAsync(buffer, leftoverCount, maxBytesToRead, cancellationToken)
                    .ConfigureAwait(false);

                if (bytesRead == 0)
                {
                    break; // EOF
                }

                totalBytesRead += bytesRead;

                // Running size check (for non-seekable streams especially)
                if (maxAllowedSize.HasValue && totalBytesRead > maxAllowedSize.Value)
                {
                    throw new InvalidOperationException(
                        $"Stream size exceeds maximum allowed size for {mimeType} ({maxAllowedSize.Value} bytes).");
                }

                var totalInBuffer = leftoverCount + bytesRead;
                var bytesToEncode = (totalInBuffer / 3) * 3; // multiple of 3

                if (bytesToEncode > 0)
                {
                    // Encode full 3-byte groups
                    var charsWritten = Convert.ToBase64CharArray(
                        buffer,
                        0,
                        bytesToEncode,
                        charBuffer,
                        0,
                        Base64FormattingOptions.None);

                    sb.Append(charBuffer, 0, charsWritten);
                }

                // Keep 0–2 leftover bytes for the next round
                leftoverCount = totalInBuffer - bytesToEncode;
                if (leftoverCount > 0)
                {
                    Buffer.BlockCopy(buffer, bytesToEncode, buffer, 0, leftoverCount);
                }
            }

            // Final leftover (1–2 bytes) → encode with '=' padding
            if (leftoverCount > 0)
            {
                var charsWritten = Convert.ToBase64CharArray(
                    buffer,
                    0,
                    leftoverCount,
                    charBuffer,
                    0,
                    Base64FormattingOptions.None);

                sb.Append(charBuffer, 0, charsWritten);
            }

            return sb.ToString();
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer, clearArray: false);
            ArrayPool<char>.Shared.Return(charBuffer, clearArray: false);
        }
    }
}
