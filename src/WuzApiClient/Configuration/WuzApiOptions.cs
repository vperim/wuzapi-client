using System;
using WuzApiClient.Exceptions;

namespace WuzApiClient.Configuration;

/// <summary>
/// Configuration options for WuzAPI gateway connection.
/// Contains server-level settings only. Authentication tokens are passed
/// at runtime via <see cref="Core.Interfaces.IWaClientFactory"/> and
/// <see cref="Core.Interfaces.IWuzApiAdminClientFactory"/>.
/// </summary>
public sealed class WuzApiOptions
{
    /// <summary>
    /// Configuration section name in appsettings.json.
    /// </summary>
    public const string SectionName = "WuzApi";

    /// <summary>
    /// Gets or sets the base URL of the WuzAPI gateway.
    /// </summary>
    /// <example>http://localhost:8080/</example>
    public string BaseUrl { get; set; } = "http://localhost:8080/";

    /// <summary>
    /// Gets or sets the request timeout in seconds.
    /// Default: 30 seconds.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Gets the configured timeout as a <see cref="TimeSpan"/>.
    /// </summary>
    public TimeSpan Timeout => TimeSpan.FromSeconds(this.TimeoutSeconds);

    /// <summary>
    /// Validates the configuration options.
    /// </summary>
    /// <exception cref="WuzApiConfigurationException">Thrown when configuration is invalid.</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(this.BaseUrl))
        {
            throw new WuzApiConfigurationException("BaseUrl is required.");
        }

        if (!Uri.TryCreate(this.BaseUrl, UriKind.Absolute, out var uri))
        {
            throw new WuzApiConfigurationException("BaseUrl must be a valid absolute URI.");
        }

        if (uri.Scheme != "http" && uri.Scheme != "https")
        {
            throw new WuzApiConfigurationException("BaseUrl must use http or https scheme.");
        }

        if (this.TimeoutSeconds <= 0)
        {
            throw new WuzApiConfigurationException("TimeoutSeconds must be a positive value.");
        }
    }
}
