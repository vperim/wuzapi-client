using System.IO;
using System.Text;
using System.Text.Json;

namespace WuzApiClient.RabbitMq.Serialization;

/// <summary>
/// Helper for merging JSON elements from wuzapi event structure.
/// Combines event object properties with root-level wuzapi-added fields.
/// </summary>
internal static class JsonMergeHelper
{
    /// <summary>
    /// Builds merged JSON by combining event object with root-level wuzapi fields.
    /// </summary>
    /// <param name="eventElement">The "event" JSON element.</param>
    /// <param name="rootElement">The root JSON element.</param>
    /// <returns>Merged JSON string for deserialization.</returns>
    public static string BuildEventDataJson(JsonElement eventElement, JsonElement rootElement)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            writer.WriteStartObject();

            // First, copy all properties from "event" object if it exists
            if (eventElement.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in eventElement.EnumerateObject())
                {
                    property.WriteTo(writer);
                }
            }

            // Then, copy root-level properties (wuzapi-added fields), excluding envelope fields
            foreach (var property in rootElement.EnumerateObject())
            {
                if (property.Name is "type" or "userID" or "instanceName" or "event")
                {
                    continue;
                }

                property.WriteTo(writer);
            }

            writer.WriteEndObject();
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }
}
