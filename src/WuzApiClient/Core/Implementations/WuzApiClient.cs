using System.Net.Http;
using Microsoft.Extensions.Options;
using WuzApiClient.Configuration;
using WuzApiClient.Core.Interfaces;
using WuzApiClient.Core.Internal;

namespace WuzApiClient.Core.Implementations;

/// <summary>
/// Main client implementation for WuzAPI operations.
/// This is a partial class - method implementations are in separate files.
/// </summary>
public sealed partial class WuzApiClient : IWuzApiClient
{
    private const string TokenHeader = "Token";

    private readonly WuzApiHttpClient httpClient;
    private readonly IOptions<WuzApiOptions> options;

    /// <summary>
    /// Initializes a new instance of the <see cref="WuzApiClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="options">The configuration options.</param>
    public WuzApiClient(HttpClient httpClient, IOptions<WuzApiOptions> options)
    {
        this.httpClient = new WuzApiHttpClient(httpClient);
        this.options = options;
    }

    private string UserToken => this.options.Value.UserToken;
}
