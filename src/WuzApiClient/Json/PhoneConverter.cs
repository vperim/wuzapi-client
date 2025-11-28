using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using WuzApiClient.Models.Common;

namespace WuzApiClient.Json;

/// <summary>
/// JSON converter for <see cref="Phone"/> type.
/// </summary>
public sealed class PhoneConverter : JsonConverter<Phone>
{
    /// <inheritdoc/>
    public override Phone Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (Phone.TryCreate(value, out var phone))
            return phone;

        throw new JsonException($"Invalid phone number format: {value}");
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, Phone value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
