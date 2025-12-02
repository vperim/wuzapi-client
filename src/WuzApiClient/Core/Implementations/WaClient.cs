using System;
using System.Net.Http;
using WuzApiClient.Core.Interfaces;
using WuzApiClient.Core.Internal;

namespace WuzApiClient.Core.Implementations;

/// <summary>
/// Main client implementation for WuzAPI operations.
/// This is a partial class - method implementations are in separate files.
/// </summary>
/// <remarks>
/// Instances should be created via <see cref="IWaClientFactory"/> for proper
/// HttpClient lifecycle management. Direct instantiation is supported for
/// testing or manual DI scenarios.
/// </remarks>
public sealed partial class WaClient : IWaClient
{
    private readonly WuzApiHttpClient httpClient;
    private readonly string userToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="WaClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client configured with base URL and timeout.</param>
    /// <param name="userToken">The user token for authentication.</param>
    /// <exception cref="ArgumentNullException">Thrown when httpClient is null.</exception>
    /// <exception cref="ArgumentException">Thrown when userToken is null or whitespace.</exception>
    public WaClient(HttpClient httpClient, string userToken)
    {
        if (httpClient == null)
            throw new ArgumentNullException(nameof(httpClient));
        if (string.IsNullOrWhiteSpace(userToken))
            throw new ArgumentException("User token is required.", nameof(userToken));

        this.httpClient = new WuzApiHttpClient(httpClient);
        this.userToken = userToken;
    }

    private string UserToken => this.userToken;
}
