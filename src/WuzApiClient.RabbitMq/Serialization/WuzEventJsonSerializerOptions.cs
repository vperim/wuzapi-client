using System.Text.Json;
using System.Text.Json.Serialization;

namespace WuzApiClient.RabbitMq.Serialization;

/// <summary>
/// JSON serializer options for WuzEvents.
/// </summary>
public static class WuzEventJsonSerializerOptions
{
    /// <summary>
    /// Gets the default serializer options.
    /// </summary>
    public static JsonSerializerOptions Default { get; } = CreateDefault();

    private static JsonSerializerOptions CreateDefault()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        options.Converters.Add(new JsonStringEnumConverter());
        options.Converters.Add(new WuzEventJsonConverter());

        return options;
    }
}
