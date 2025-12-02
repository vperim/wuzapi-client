using System;
using System.Net.Http;
using WuzApiClient.Core.Interfaces;

namespace WuzApiClient.Core.Implementations;

/// <summary>
/// Factory implementation for creating <see cref="WuzApiAdminClient"/> instances.
/// </summary>
internal sealed class WuzApiAdminClientFactory : IWuzApiAdminClientFactory
{
    private readonly IHttpClientFactory httpClientFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="WuzApiAdminClientFactory"/> class.
    /// </summary>
    /// <param name="httpClientFactory">The HTTP client factory.</param>
    public WuzApiAdminClientFactory(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    /// <inheritdoc/>
    public IWuzApiAdminClient CreateClient(string adminToken)
    {
        var httpClient = this.httpClientFactory.CreateClient(WaClientFactory.HttpClientName);
        return new WuzApiAdminClient(httpClient, adminToken);
    }
}
