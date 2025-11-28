using System.Text.Json.Serialization;
using WuzApiClient.Models.Common;

namespace WuzApiClient.Models.Requests.User;

/// <summary>
/// Request to check if phone numbers have WhatsApp.
/// </summary>
public sealed class CheckPhonesRequest
{
    /// <summary>
    /// Gets or sets the phone numbers to check.
    /// </summary>
    [JsonPropertyName("Phone")]
    public Phone[] Phones { get; set; } = [];
}
