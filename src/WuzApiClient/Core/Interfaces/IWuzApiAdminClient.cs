using System.Threading;
using System.Threading.Tasks;
using WuzApiClient.Models.Requests.Admin;
using WuzApiClient.Models.Responses.Admin;
using WuzApiClient.Results;

namespace WuzApiClient.Core.Interfaces;

/// <summary>
/// Admin client for managing WuzAPI users (instances).
/// </summary>
public interface IWuzApiAdminClient
{
    /// <summary>
    /// Lists all users (instances).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing user list or error.</returns>
    Task<WuzResult<UserListResponse>> ListUsersAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new user (instance).
    /// </summary>
    /// <param name="request">The create user request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing the created user or error.</returns>
    Task<WuzResult<UserResponse>> CreateUserAsync(
        CreateUserRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a user (instance).
    /// </summary>
    /// <param name="userId">The user ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult indicating success or failure.</returns>
    Task<WuzResult> DeleteUserAsync(
        string userId,
        CancellationToken cancellationToken = default);
}
