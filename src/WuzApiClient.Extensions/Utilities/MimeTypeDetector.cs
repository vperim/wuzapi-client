using System.Collections.Generic;
using System.IO;

namespace WuzApiClient.Extensions.Utilities;

/// <summary>
/// Internal utility for detecting MIME types from file extensions.
/// </summary>
internal static class MimeTypeDetector
{
    private static readonly Dictionary<string, string> ExtensionMap = new()
    {
        // WhatsApp-Compatible Image Types
        { ".jpg", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".png", "image/png" },
        { ".gif", "image/gif" },
        { ".webp", "image/webp" },

        // Common Document Types
        { ".pdf", "application/pdf" },
        { ".doc", "application/msword" },
        { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
        { ".xls", "application/vnd.ms-excel" },
        { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
        { ".ppt", "application/vnd.ms-powerpoint" },
        { ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
        { ".txt", "text/plain" },

        // WhatsApp-Compatible Audio Types
        { ".mp3", "audio/mpeg" },
        { ".ogg", "audio/ogg" },
        { ".oga", "audio/ogg" },
        { ".m4a", "audio/mp4" },
        { ".aac", "audio/aac" },
        { ".opus", "audio/ogg" },

        // WhatsApp-Compatible Video Types
        { ".mp4", "video/mp4" },
        { ".3gp", "video/3gpp" },
        { ".3gpp", "video/3gpp" }
    };

    /// <summary>
    /// Detects MIME type from file extension.
    /// </summary>
    /// <param name="filePath">File path or name.</param>
    /// <returns>MIME type or application/octet-stream if unknown.</returns>
    public static string DetectFromExtension(string filePath)
    {
        var extension = Path.GetExtension(filePath)?.ToLowerInvariant();

        if (extension is null)
        {
            return "application/octet-stream";
        }

        return ExtensionMap.TryGetValue(extension, out var mimeType)
            ? mimeType
            : "application/octet-stream";
    }
}
