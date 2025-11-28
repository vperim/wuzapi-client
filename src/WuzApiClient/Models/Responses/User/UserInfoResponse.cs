using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WuzApiClient.Models.Responses.User;

/// <summary>
/// Response containing user information.
/// The API returns users as a dictionary with JID as key.
/// </summary>
public sealed class UserInfoResponse
{
    /// <summary>
    /// Gets or sets the users dictionary (JID -> UserInfo).
    /// </summary>
    [JsonPropertyName("Users")]
    public Dictionary<string, UserInfo> Users { get; set; } = new();
}

/// <summary>
/// User information details.
/// </summary>
public sealed class UserInfo
{
    /// <summary>
    /// Gets or sets the user's devices.
    /// </summary>
    [JsonPropertyName("Devices")]
    public string[]? Devices { get; set; }

    /// <summary>
    /// Gets or sets the user's LID (local ID).
    /// </summary>
    [JsonPropertyName("LID")]
    public string? Lid { get; set; }

    /// <summary>
    /// Gets or sets the user's profile picture ID.
    /// </summary>
    [JsonPropertyName("PictureID")]
    public string? PictureId { get; set; }

    /// <summary>
    /// Gets or sets the user's status.
    /// </summary>
    [JsonPropertyName("Status")]
    public string? Status { get; set; }

    /// <summary>
    /// Gets or sets the verified name information.
    /// </summary>
    [JsonPropertyName("VerifiedName")]
    public VerifiedNameInfo? VerifiedName { get; set; }
}

/// <summary>
/// Verified business name information.
/// </summary>
public sealed class VerifiedNameInfo
{
    /// <summary>
    /// Gets or sets the certificate details.
    /// </summary>
    [JsonPropertyName("Certificate")]
    public CertificateDetails? Certificate { get; set; }

    /// <summary>
    /// Gets or sets the verification details.
    /// </summary>
    [JsonPropertyName("Details")]
    public VerificationDetails? Details { get; set; }
}

/// <summary>
/// Certificate details for verified business.
/// </summary>
public sealed class CertificateDetails
{
    /// <summary>
    /// Gets or sets the certificate details string.
    /// </summary>
    [JsonPropertyName("details")]
    public string? Details { get; set; }

    /// <summary>
    /// Gets or sets the signature.
    /// </summary>
    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}

/// <summary>
/// Verification details for verified business.
/// </summary>
public sealed class VerificationDetails
{
    /// <summary>
    /// Gets or sets the issuer.
    /// </summary>
    [JsonPropertyName("issuer")]
    public string? Issuer { get; set; }

    /// <summary>
    /// Gets or sets the serial number.
    /// </summary>
    [JsonPropertyName("serial")]
    public long Serial { get; set; }

    /// <summary>
    /// Gets or sets the verified name.
    /// </summary>
    [JsonPropertyName("verifiedName")]
    public string? VerifiedName { get; set; }
}
