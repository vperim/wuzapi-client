using System;
using System.Text.Json;
using WuzApiClient.Common.Results;
using WuzApiClient.Common.Extensions;

namespace WuzApiClient.Core.Internal;

/// <summary>
/// Helper for parsing WuzAPI responses.
/// </summary>
internal static class ResponseParser
{
    /// <summary>
    /// Parses a JSON response into the specified type.
    /// Handles the asternic/wuzapi response format which may wrap data in a "data" property.
    /// </summary>
    /// <typeparam name="T">The expected response type.</typeparam>
    /// <param name="rawContent">The raw JSON content.</param>
    /// <param name="jsonOptions">The JSON serializer options.</param>
    /// <returns>WuzResult containing the parsed response or error.</returns>
    public static WuzResult<T> Parse<T>(string rawContent, JsonSerializerOptions jsonOptions)
    {
        if (string.IsNullOrWhiteSpace(rawContent))
        {
            return WuzApiError.DeserializationError("Response body is empty.");
        }

        try
        {
            using var doc = JsonDocument.Parse(rawContent);
            var root = doc.RootElement;

            // First, try to extract from "data" property (asternic/wuzapi pattern)
            if (root.TryGetProperty("data", out var dataElement))
            {
                var data = dataElement.Deserialize<T>(jsonOptions);
                if (data is not null)
                {
                    return WuzResult<T>.Success(data);
                }
            }

            // Also try "Data" (PascalCase variant)
            if (root.TryGetProperty("Data", out var dataElementPascal))
            {
                var data = dataElementPascal.Deserialize<T>(jsonOptions);
                if (data is not null)
                {
                    return WuzResult<T>.Success(data);
                }
            }

            // Some endpoints return data directly without wrapper
            var directData = root.Deserialize<T>(jsonOptions);
            if (directData is not null)
            {
                return WuzResult<T>.Success(directData);
            }

            return WuzApiError.DeserializationError("Response data is null or could not be parsed.");
        }
        catch (JsonException ex)
        {
            return WuzApiError.DeserializationError($"JSON parsing error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return WuzApiError.DeserializationError($"Unexpected error parsing response: {ex.Message}");
        }
    }

    /// <summary>
    /// Extracts an error message from a JSON error response.
    /// </summary>
    /// <param name="rawContent">The raw JSON content.</param>
    /// <returns>The error message if found; otherwise, null.</returns>
    public static string? ExtractErrorMessage(string? rawContent)
    {
        if (rawContent.IsNullOrWhiteSpace())
            return null;

        try
        {
            using var doc = JsonDocument.Parse(rawContent);
            var root = doc.RootElement;

            // Try "error" property
            if (root.TryGetProperty("error", out var errorElement))
            {
                return errorElement.GetString();
            }

            // Try "Error" (PascalCase)
            if (root.TryGetProperty("Error", out var errorElementPascal))
            {
                return errorElementPascal.GetString();
            }

            // Try "message" property
            if (root.TryGetProperty("message", out var messageElement))
            {
                return messageElement.GetString();
            }

            // Try "Message" (PascalCase)
            if (root.TryGetProperty("Message", out var messageElementPascal))
            {
                return messageElementPascal.GetString();
            }
        }
        catch
        {
            // Ignore parsing errors
        }

        return null;
    }
}
