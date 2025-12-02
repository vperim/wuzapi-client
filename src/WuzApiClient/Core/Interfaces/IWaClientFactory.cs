namespace WuzApiClient.Core.Interfaces;

/// <summary>
/// Factory for creating <see cref="IWaClient"/> instances with dynamic user tokens.
/// </summary>
/// <remarks>
/// Use this factory when managing multiple WhatsApp accounts. Each call to
/// <see cref="CreateClient"/> returns a new client instance configured with
/// the specified user token while sharing the underlying HTTP handler pool.
/// </remarks>
public interface IWaClientFactory
{
    /// <summary>
    /// Creates a new <see cref="IWaClient"/> configured with the specified user token.
    /// </summary>
    /// <param name="userToken">The user token for authentication.</param>
    /// <returns>A configured <see cref="IWaClient"/> instance.</returns>
    IWaClient CreateClient(string userToken);
}
