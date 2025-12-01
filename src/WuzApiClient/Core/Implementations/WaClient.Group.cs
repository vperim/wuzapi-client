using System.Threading;
using System.Threading.Tasks;
using WuzApiClient.Models.Requests.Group;
using WuzApiClient.Models.Responses.Group;
using WuzApiClient.Results;

namespace WuzApiClient.Core.Implementations;

// Group management methods - to be implemented by Batch 2 Stream B
public sealed partial class WaClient
{
    /// <inheritdoc/>
    public async Task<WuzResult<GroupListResponse>> GetGroupsAsync(
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.GetAsync<GroupListResponse>(
            "/group/list",
            "Token",
            this.UserToken,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult<GroupInfoResponse>> CreateGroupAsync(
        CreateGroupRequest request,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.PostAsync<GroupInfoResponse>(
            "/group/create",
            "Token",
            this.UserToken,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult<GroupInfoResponse>> GetGroupInfoAsync(
        string groupId,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.GetAsync<GroupInfoResponse>(
            $"/group/info?groupJID={groupId}",
            "Token",
            this.UserToken,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult<GroupInviteLinkResponse>> GetGroupInviteLinkAsync(
        string groupId,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.GetAsync<GroupInviteLinkResponse>(
            $"/group/invitelink?groupJID={groupId}",
            "Token",
            this.UserToken,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult> UpdateGroupPhotoAsync(
        UpdateGroupPhotoRequest request,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.PostAsync(
            "/group/photo",
            "Token",
            this.UserToken,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult> RemoveGroupPhotoAsync(
        string groupId,
        CancellationToken cancellationToken = default)
    {
        var request = new RemoveGroupPhotoRequest { GroupId = groupId };

        return await this.httpClient.PostAsync(
            "/group/photo/remove",
            "Token",
            this.UserToken,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult> UpdateGroupNameAsync(
        UpdateGroupNameRequest request,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.PostAsync(
            "/group/name",
            "Token",
            this.UserToken,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult> UpdateGroupTopicAsync(
        UpdateGroupTopicRequest request,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.PostAsync(
            "/group/topic",
            "Token",
            this.UserToken,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult> SetGroupAnnounceAsync(
        SetGroupAnnounceRequest request,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.PostAsync(
            "/group/announce",
            "Token",
            this.UserToken,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult> ManageParticipantsAsync(
        ManageParticipantsRequest request,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.PostAsync(
            "/group/participants",
            "Token",
            this.UserToken,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult> SetGroupLockedAsync(
        SetGroupLockRequest request,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.PostAsync(
            "/group/lock",
            "Token",
            this.UserToken,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult> SetDisappearingMessagesAsync(
        SetDisappearingMessagesRequest request,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.PostAsync(
            "/group/disappearing",
            "Token",
            this.UserToken,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult<GroupInfoResponse>> JoinGroupAsync(
        JoinGroupRequest request,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.PostAsync<GroupInfoResponse>(
            "/group/join",
            "Token",
            this.UserToken,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult<GroupInviteInfoResponse>> GetInviteInfoAsync(
        GetInviteInfoRequest request,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.PostAsync<GroupInviteInfoResponse>(
            "/group/inviteinfo",
            "Token",
            this.UserToken,
            request,
            cancellationToken).ConfigureAwait(false);
    }
}
