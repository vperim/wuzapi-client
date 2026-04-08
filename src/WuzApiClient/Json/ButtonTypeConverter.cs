using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using WuzApiClient.Models.Common;

namespace WuzApiClient.Json;

/// <summary>
/// JSON converter for ButtonType enum that serializes using snake_case values
/// matching the wuzapi server expectations (e.g. "cta_url", "cta_call").
/// Required because the global JsonStringEnumConverter uses camelCase which
/// would produce "ctaUrl" instead of the expected "cta_url".
/// Note: adding a new ButtonType value requires updating both switch blocks here.
/// </summary>
public sealed class ButtonTypeConverter : JsonConverter<ButtonType>
{
    /// <summary>
    /// Reads a JSON string and converts it to a ButtonType enum value.
    /// </summary>
    public override ButtonType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException($"Expected string token for ButtonType, got {reader.TokenType}");

        var value = reader.GetString() ?? string.Empty;

        switch (value)
        {
            case "reply": return ButtonType.Reply;
            case "cta_url": return ButtonType.CtaUrl;
            case "cta_call": return ButtonType.CtaCall;
            case "copy": return ButtonType.Copy;
            default: throw new JsonException($"Unknown ButtonType value: {value}");
        }
    }

    /// <summary>
    /// Writes the ButtonType enum as a JSON string using snake_case format.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, ButtonType value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case ButtonType.Reply:
                writer.WriteStringValue("reply");
                break;
            case ButtonType.CtaUrl:
                writer.WriteStringValue("cta_url");
                break;
            case ButtonType.CtaCall:
                writer.WriteStringValue("cta_call");
                break;
            case ButtonType.Copy:
                writer.WriteStringValue("copy");
                break;
            default:
                throw new JsonException($"Unknown ButtonType value: {value}");
        }
    }
}
