using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using WuzApiClient.Common.DataTypes;

namespace WuzApiClient.Common.Serialization;

/// <summary>
/// JSON converter for <see cref="DataUri"/> that serializes/deserializes as a string.
/// </summary>
public sealed class DataUriJsonConverter : JsonConverter<DataUri?>
{
    public override DataUri? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (string.IsNullOrEmpty(value))
            return null;

        // Use TryParse to gracefully handle invalid data URIs
        if (DataUri.TryParse(value, out var result))
            return result;

        // If not a valid data URI, return null rather than throwing
        return null;
    }

    public override void Write(Utf8JsonWriter writer, DataUri? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
