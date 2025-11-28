using System.Net;
using System.Text;
using System.Text.Json;
using WuzApiClient.Json;

namespace WuzApiClient.UnitTests.TestInfrastructure.Mocks;

/// <summary>
/// Mock HTTP message handler for testing HTTP client interactions.
/// </summary>
public sealed class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly Queue<HttpResponseMessage> responseQueue = new();
    private readonly List<HttpRequestMessage> receivedRequests = [];
    private readonly List<string?> receivedRequestContents = [];
    private bool disposed;

    /// <summary>
    /// Gets the list of received HTTP requests.
    /// </summary>
    public IReadOnlyList<HttpRequestMessage> ReceivedRequests => this.receivedRequests;

    /// <summary>
    /// Gets the list of received request body contents (captured before disposal).
    /// </summary>
    public IReadOnlyList<string?> ReceivedRequestContents => this.receivedRequestContents;

    /// <summary>
    /// Gets the number of responses remaining in the queue.
    /// </summary>
    public int ResponseQueueCount => this.responseQueue.Count;

    /// <summary>
    /// Enqueues a response with the specified status code and content.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="content">The response content as a string.</param>
    public void EnqueueResponse(HttpStatusCode statusCode, string content)
    {
        var response = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(content, Encoding.UTF8, "application/json")
        };
        this.responseQueue.Enqueue(response);
    }

    /// <summary>
    /// Enqueues a success response with the data wrapped in the standard WuzAPI format.
    /// </summary>
    /// <typeparam name="T">The type of data to serialize.</typeparam>
    /// <param name="data">The data to include in the response.</param>
    public void EnqueueSuccessResponse<T>(T data)
    {
        var wrapper = new { data };
        var json = JsonSerializer.Serialize(wrapper, WuzApiJsonSerializerOptions.Default);
        this.EnqueueResponse(HttpStatusCode.OK, json);
    }

    /// <summary>
    /// Enqueues an error response with the specified status code and error message.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="errorMessage">The optional error message.</param>
    public void EnqueueErrorResponse(HttpStatusCode statusCode, string? errorMessage = null)
    {
        var wrapper = new { error = errorMessage ?? "An error occurred" };
        var json = JsonSerializer.Serialize(wrapper, WuzApiJsonSerializerOptions.Default);
        this.EnqueueResponse(statusCode, json);
    }

    /// <inheritdoc/>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(this.disposed, this);

        this.receivedRequests.Add(request);

        // Capture request content before it gets disposed
        string? content = null;
        if (request.Content is not null)
        {
            content = await request.Content.ReadAsStringAsync(cancellationToken);
        }
        this.receivedRequestContents.Add(content);

        if (this.responseQueue.Count == 0)
        {
            throw new InvalidOperationException(
                "No responses queued. Call EnqueueResponse, EnqueueSuccessResponse, or EnqueueErrorResponse before making requests.");
        }

        var response = this.responseQueue.Dequeue();
        return response;
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (this.disposed)
        {
            return;
        }

        if (disposing)
        {
            foreach (var response in this.responseQueue)
            {
                response.Dispose();
            }

            this.responseQueue.Clear();
        }

        this.disposed = true;
        base.Dispose(disposing);
    }
}
