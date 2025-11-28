using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WuzApiClient.Json;
using WuzApiClient.Results;

namespace WuzApiClient.Core.Internal;

/// <summary>
/// Internal HTTP client for making WuzAPI requests.
/// Handles serialization, deserialization, and error mapping.
/// </summary>
internal sealed class WuzApiHttpClient
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions jsonOptions;

    /// <summary>
    /// Initializes a new instance of <see cref="WuzApiHttpClient"/>.
    /// </summary>
    /// <param name="httpClient">The HTTP client to use for requests.</param>
    public WuzApiHttpClient(HttpClient httpClient)
    {
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        this.jsonOptions = WuzApiJsonSerializerOptions.Default;
    }

    /// <summary>
    /// Sends an HTTP request and returns a typed result.
    /// </summary>
    /// <typeparam name="T">The expected response type.</typeparam>
    /// <param name="method">The HTTP method.</param>
    /// <param name="path">The request path (relative to base URL).</param>
    /// <param name="authHeader">The authentication header name.</param>
    /// <param name="authValue">The authentication header value.</param>
    /// <param name="body">Optional request body.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing the response or error.</returns>
    public async Task<WuzResult<T>> SendAsync<T>(
        HttpMethod method,
        string path,
        string authHeader,
        string authValue,
        object? body,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, path);
        request.Headers.TryAddWithoutValidation(authHeader, authValue);

        if (body is not null)
        {
            var json = JsonSerializer.Serialize(body, this.jsonOptions);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        HttpResponseMessage response;
        try
        {
            response = await this.httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw; // Let cancellation propagate
        }
        catch (TaskCanceledException)
        {
            return WuzApiError.Timeout();
        }
        catch (HttpRequestException ex)
        {
            return WuzApiError.NetworkError(ex.Message);
        }

        string rawContent;
        try
        {
            rawContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return WuzApiError.NetworkError($"Failed to read response: {ex.Message}");
        }

        if (!response.IsSuccessStatusCode)
        {
            return WuzApiError.FromHttpStatus((int)response.StatusCode, rawContent);
        }

        return ResponseParser.Parse<T>(rawContent, this.jsonOptions);
    }

    /// <summary>
    /// Sends an HTTP request without expecting a response body.
    /// </summary>
    /// <param name="method">The HTTP method.</param>
    /// <param name="path">The request path (relative to base URL).</param>
    /// <param name="authHeader">The authentication header name.</param>
    /// <param name="authValue">The authentication header value.</param>
    /// <param name="body">Optional request body.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult indicating success or failure.</returns>
    public async Task<WuzResult> SendAsync(
        HttpMethod method,
        string path,
        string authHeader,
        string authValue,
        object? body,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, path);
        request.Headers.TryAddWithoutValidation(authHeader, authValue);

        if (body is not null)
        {
            var json = JsonSerializer.Serialize(body, this.jsonOptions);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        HttpResponseMessage response;
        try
        {
            response = await this.httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw; // Let cancellation propagate
        }
        catch (TaskCanceledException)
        {
            return WuzApiError.Timeout();
        }
        catch (HttpRequestException ex)
        {
            return WuzApiError.NetworkError(ex.Message);
        }

        if (!response.IsSuccessStatusCode)
        {
            string rawContent;
            try
            {
                rawContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            catch
            {
                rawContent = string.Empty;
            }

            return WuzApiError.FromHttpStatus((int)response.StatusCode, rawContent);
        }

        return WuzResult.Success();
    }

    /// <summary>
    /// Sends an HTTP GET request with query parameters.
    /// </summary>
    /// <typeparam name="T">The expected response type.</typeparam>
    /// <param name="path">The request path with query string.</param>
    /// <param name="authHeader">The authentication header name.</param>
    /// <param name="authValue">The authentication header value.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing the response or error.</returns>
    public Task<WuzResult<T>> GetAsync<T>(
        string path,
        string authHeader,
        string authValue,
        CancellationToken cancellationToken)
    {
        return this.SendAsync<T>(HttpMethod.Get, path, authHeader, authValue, null, cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP POST request.
    /// </summary>
    /// <typeparam name="T">The expected response type.</typeparam>
    /// <param name="path">The request path.</param>
    /// <param name="authHeader">The authentication header name.</param>
    /// <param name="authValue">The authentication header value.</param>
    /// <param name="body">The request body.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing the response or error.</returns>
    public Task<WuzResult<T>> PostAsync<T>(
        string path,
        string authHeader,
        string authValue,
        object? body,
        CancellationToken cancellationToken)
    {
        return this.SendAsync<T>(HttpMethod.Post, path, authHeader, authValue, body, cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP POST request without expecting a response body.
    /// </summary>
    /// <param name="path">The request path.</param>
    /// <param name="authHeader">The authentication header name.</param>
    /// <param name="authValue">The authentication header value.</param>
    /// <param name="body">The request body.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult indicating success or failure.</returns>
    public Task<WuzResult> PostAsync(
        string path,
        string authHeader,
        string authValue,
        object? body,
        CancellationToken cancellationToken)
    {
        return this.SendAsync(HttpMethod.Post, path, authHeader, authValue, body, cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP DELETE request.
    /// </summary>
    /// <param name="path">The request path.</param>
    /// <param name="authHeader">The authentication header name.</param>
    /// <param name="authValue">The authentication header value.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult indicating success or failure.</returns>
    public Task<WuzResult> DeleteAsync(
        string path,
        string authHeader,
        string authValue,
        CancellationToken cancellationToken)
    {
        return this.SendAsync(HttpMethod.Delete, path, authHeader, authValue, null, cancellationToken);
    }
}
