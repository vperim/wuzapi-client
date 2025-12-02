using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WuzApiClient.Core.Interfaces;
using WuzApiClient.Core.Internal;
using WuzApiClient.Models.Requests.Admin;
using WuzApiClient.Models.Responses.Admin;
using WuzApiClient.Results;

namespace WuzApiClient.Core.Implementations;

/// <summary>
/// Admin client implementation for managing WuzAPI users.
/// </summary>
/// <remarks>
/// Instances should be created via <see cref="IWuzApiAdminClientFactory"/> for proper
/// HttpClient lifecycle management. Direct instantiation is supported for
/// testing or manual DI scenarios.
/// </remarks>
public sealed class WuzApiAdminClient : IWuzApiAdminClient
{
    private const string AuthHeader = "Authorization";

    private readonly WuzApiHttpClient httpClient;
    private readonly string adminToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="WuzApiAdminClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client configured with base URL and timeout.</param>
    /// <param name="adminToken">The admin token for authentication.</param>
    /// <exception cref="ArgumentNullException">Thrown when httpClient is null.</exception>
    /// <exception cref="ArgumentException">Thrown when adminToken is null or whitespace.</exception>
    public WuzApiAdminClient(HttpClient httpClient, string adminToken)
    {
        if (httpClient == null)
            throw new ArgumentNullException(nameof(httpClient));
        if (string.IsNullOrWhiteSpace(adminToken))
            throw new ArgumentException("Admin token is required.", nameof(adminToken));

        this.httpClient = new WuzApiHttpClient(httpClient);
        this.adminToken = adminToken;
    }

    private string AdminToken => this.adminToken;

    /// <inheritdoc/>
    public async Task<WuzResult<UserListResponse>> ListUsersAsync(CancellationToken cancellationToken = default)
    {
        // The admin API returns the user array directly in the data field,
        // so we deserialize to UserResponse[] and wrap it.
        var result = await this.httpClient.GetAsync<UserResponse[]>(
            "/admin/users",
            AuthHeader,
            this.AdminToken,
            cancellationToken);

        return result.Match(
            users => WuzResult<UserListResponse>.Success(UserListResponse.FromArray(users)),
            error => WuzResult<UserListResponse>.Failure(error));
    }

    /// <inheritdoc/>
    public Task<WuzResult<UserResponse>> CreateUserAsync(
        CreateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        return this.httpClient.PostAsync<UserResponse>(
            "/admin/users",
            AuthHeader,
            this.AdminToken,
            request,
            cancellationToken);
    }

    /// <inheritdoc/>
    public Task<WuzResult> DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        return this.httpClient.DeleteAsync(
            $"/admin/users/{userId}",
            AuthHeader,
            this.AdminToken,
            cancellationToken);
    }
}
