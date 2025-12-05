using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WuzApiClient.Common.Results;
using WuzApiClient.Models.Common;
using WuzApiClient.Models.Requests.User;
using WuzApiClient.Models.Responses.User;

namespace WuzApiClient.Core.Implementations;

// User information methods
public sealed partial class WaClient
{
    /// <inheritdoc/>
    public async Task<WuzResult<UserInfoResponse>> GetUserInfoAsync(
        Phone phone,
        CancellationToken cancellationToken = default)
    {
        // User/info endpoint requires JID format
        var request = new GetUserInfoRequest { Phones = [phone.ToJid()] };

        return await this.httpClient.PostAsync<UserInfoResponse>(
            "/user/info",
            "Token",
            this.UserToken,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult<CheckPhonesResponse>> CheckPhonesAsync(
        Phone[] phones,
        CancellationToken cancellationToken = default)
    {
        var request = new CheckPhonesRequest { Phones = phones };

        return await this.httpClient.PostAsync<CheckPhonesResponse>(
            "/user/check",
            "Token",
            this.UserToken,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult<AvatarResponse>> GetAvatarAsync(
        Phone phone,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.GetAsync<AvatarResponse>(
            $"/user/avatar?Phone={phone.Value}",
            "Token",
            this.UserToken,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult<ContactsResponse>> GetContactsAsync(
        CancellationToken cancellationToken = default)
    {
        // The API returns contacts as a dictionary directly in the data field
        var result = await this.httpClient.GetAsync<Dictionary<string, ContactInfo>>(
            "/user/contacts",
            "Token",
            this.UserToken,
            cancellationToken).ConfigureAwait(false);

        return result.Match(
            contacts => WuzResult<ContactsResponse>.Success(ContactsResponse.FromDictionary(contacts)),
            error => WuzResult<ContactsResponse>.Failure(error));
    }
}
