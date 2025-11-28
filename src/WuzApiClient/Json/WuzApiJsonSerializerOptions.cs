using System.Text.Json;
using System.Text.Json.Serialization;

namespace WuzApiClient.Json;

/// <summary>
/// Provides pre-configured JSON serializer options for WuzAPI client.
/// </summary>
public static class WuzApiJsonSerializerOptions
{
    private static JsonSerializerOptions? defaultOptions;

    /// <summary>
    /// Gets the default JSON serializer options configured for WuzAPI.
    /// </summary>
    public static JsonSerializerOptions Default => defaultOptions ??= CreateDefaultOptions();

    /// <summary>
    /// Creates a new instance of JSON serializer options configured for WuzAPI.
    /// </summary>
    /// <returns>Configured <see cref="JsonSerializerOptions"/>.</returns>
    public static JsonSerializerOptions CreateDefaultOptions()
    {
        var options = new JsonSerializerOptions
        {
            // Handle both camelCase and PascalCase from API responses
            PropertyNameCaseInsensitive = true,

            // Use camelCase for outgoing requests (WuzAPI expects camelCase)
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,

            // Don't serialize null values to reduce payload size
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,

            // Allow reading numbers from strings for flexibility
            NumberHandling = JsonNumberHandling.AllowReadingFromString,

            // Enable async enumerable support
            WriteIndented = false
        };

        // Add custom converters
        options.Converters.Add(new PhoneConverter());
        options.Converters.Add(new JidConverter());
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

        return options;
    }
}
