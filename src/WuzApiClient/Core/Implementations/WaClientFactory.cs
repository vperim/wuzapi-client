using System;
using System.Net.Http;
using WuzApiClient.Core.Interfaces;

namespace WuzApiClient.Core.Implementations;

/// <summary>
/// Factory implementation for creating <see cref="WaClient"/> instances.
/// </summary>
internal sealed class WaClientFactory : IWaClientFactory
{
    /// <summary>
    /// The named HttpClient registered for WuzAPI operations.
    /// </summary>
    internal const string HttpClientName = "WuzApi";

    private readonly IHttpClientFactory httpClientFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="WaClientFactory"/> class.
    /// </summary>
    /// <param name="httpClientFactory">The HTTP client factory.</param>
    public WaClientFactory(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    /// <inheritdoc/>
    public IWaClient CreateClient(string userToken)
    {
        var httpClient = this.httpClientFactory.CreateClient(HttpClientName);
        return new WaClient(httpClient, userToken);
    }
}
