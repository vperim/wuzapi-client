using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using WuzApiClient.Configuration;
using WuzApiClient.Core.Interfaces;
using WuzApiClient.Core.Internal;
using WuzApiClient.Models.Requests.Admin;
using WuzApiClient.Models.Responses.Admin;
using WuzApiClient.Results;

namespace WuzApiClient.Core.Implementations;

/// <summary>
/// Admin client implementation for managing WuzAPI users.
/// </summary>
public sealed class WuzApiAdminClient : IWuzApiAdminClient
{
    private const string AuthHeader = "Authorization";

    private readonly WuzApiHttpClient httpClient;
    private readonly IOptions<WuzApiAdminOptions> options;

    /// <summary>
    /// Initializes a new instance of the <see cref="WuzApiAdminClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="options">The configuration options.</param>
    public WuzApiAdminClient(HttpClient httpClient, IOptions<WuzApiAdminOptions> options)
    {
        this.httpClient = new WuzApiHttpClient(httpClient);
        this.options = options;
    }

    private string AdminToken => this.options.Value.AdminToken;

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
