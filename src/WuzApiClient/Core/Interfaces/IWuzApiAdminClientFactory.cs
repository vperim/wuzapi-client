namespace WuzApiClient.Core.Interfaces;

/// <summary>
/// Factory for creating <see cref="IWuzApiAdminClient"/> instances with dynamic admin tokens.
/// </summary>
/// <remarks>
/// Use this factory for administrative operations on the WuzAPI gateway.
/// Each call to <see cref="CreateClient"/> returns a new client instance
/// configured with the specified admin token while sharing the underlying
/// HTTP handler pool.
/// </remarks>
public interface IWuzApiAdminClientFactory
{
    /// <summary>
    /// Creates a new <see cref="IWuzApiAdminClient"/> configured with the specified admin token.
    /// </summary>
    /// <param name="adminToken">The admin token for authentication.</param>
    /// <returns>A configured <see cref="IWuzApiAdminClient"/> instance.</returns>
    IWuzApiAdminClient CreateClient(string adminToken);
}
