using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WuzApiClient.Common.Results;
using WuzApiClient.Models.Common;
using WuzApiClient.Models.Requests.Chat;
using WuzApiClient.Models.Requests.Group;
using WuzApiClient.Models.Requests.Session;
using WuzApiClient.Models.Requests.Webhook;
using WuzApiClient.Models.Responses.Chat;
using WuzApiClient.Models.Responses.Download;
using WuzApiClient.Models.Responses.Group;
using WuzApiClient.Models.Responses.Session;
using WuzApiClient.Models.Responses.User;
using WuzApiClient.Models.Responses.Webhook;

namespace WuzApiClient.Core.Interfaces;

/// <summary>
/// Client for interacting with asternic/wuzapi WhatsApp gateway.
/// </summary>
public interface IWaClient
{
    // ==================== SESSION MANAGEMENT ====================

    /// <summary>
    /// Connects to WhatsApp and initiates a session.
    /// </summary>
    /// <param name="request">The connection request parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult indicating success or failure.</returns>
    Task<WuzResult> ConnectSessionAsync(
        ConnectSessionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Disconnects the session while preserving authentication.
    /// The session can be reconnected without scanning QR again.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult indicating success or failure.</returns>
    Task<WuzResult> DisconnectSessionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs out and destroys the session completely.
    /// Will require new QR scan to reconnect.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult indicating success or failure.</returns>
    Task<WuzResult> LogoutSessionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current session status.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing session status or error.</returns>
    Task<WuzResult<SessionStatusResponse>> GetSessionStatusAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the QR code for authentication.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing QR code data or error.</returns>
    Task<WuzResult<QrCodeResponse>> GetQrCodeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Initiates phone pairing for authentication without QR code.
    /// </summary>
    /// <param name="request">The pairing request with phone number.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing pairing code or error.</returns>
    Task<WuzResult<PairPhoneResponse>> PairPhoneAsync(
        PairPhoneRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Configures proxy settings for the session.
    /// </summary>
    /// <param name="request">The proxy configuration request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult indicating success or failure.</returns>
    Task<WuzResult> SetProxyAsync(
        SetProxyRequest request,
        CancellationToken cancellationToken = default);

    // ==================== MESSAGING ====================

    /// <summary>
    /// Sends a text message.
    /// </summary>
    /// <param name="phone">The recipient phone number.</param>
    /// <param name="message">The message text.</param>
    /// <param name="quotedId">Optional message ID to reply to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing send response or error.</returns>
    Task<WuzResult<SendMessageResponse>> SendTextMessageAsync(
        Phone phone,
        string message,
        string? quotedId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an image message.
    /// </summary>
    /// <param name="request">The image message request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing send response or error.</returns>
    Task<WuzResult<SendMessageResponse>> SendImageAsync(
        SendImageRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an image message from a file.
    /// </summary>
    /// <param name="phone">Recipient phone number.</param>
    /// <param name="filePath">Path to the image file.</param>
    /// <param name="caption">Optional caption.</param>
    /// <param name="quotedId">Optional message ID to reply to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing send response or error.</returns>
    Task<WuzResult<SendMessageResponse>> SendImageFromFileAsync(
        Phone phone,
        string filePath,
        string? caption = null,
        string? quotedId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an image message from a stream.
    /// </summary>
    /// <param name="phone">Recipient phone number.</param>
    /// <param name="imageStream">
    /// Stream containing the image data. The caller retains ownership and is responsible
    /// for disposing the stream after this method returns.
    /// </param>
    /// <param name="mimeType">
    /// MIME type (e.g., "image/png"). If null, defaults to "image/jpeg".
    /// </param>
    /// <param name="caption">Optional caption.</param>
    /// <param name="quotedId">Optional message ID to reply to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing send response or error.</returns>
    /// <remarks>
    /// This method reads the entire stream content but does NOT dispose it.
    /// Wrap the stream in a using block or dispose it manually after calling this method.
    /// Peak memory usage is approximately 3.5x the file size during encoding.
    /// </remarks>
    Task<WuzResult<SendMessageResponse>> SendImageFromStreamAsync(
        Phone phone,
        Stream imageStream,
        string? mimeType = null,
        string? caption = null,
        string? quotedId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a document.
    /// </summary>
    /// <param name="request">The document message request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing send response or error.</returns>
    Task<WuzResult<SendMessageResponse>> SendDocumentAsync(
        SendDocumentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a document message from a file.
    /// </summary>
    /// <param name="phone">Recipient phone number.</param>
    /// <param name="filePath">Path to the document file.</param>
    /// <param name="caption">Optional caption.</param>
    /// <param name="quotedId">Optional message ID to reply to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing send response or error.</returns>
    Task<WuzResult<SendMessageResponse>> SendDocumentFromFileAsync(
        Phone phone,
        string filePath,
        string? caption = null,
        string? quotedId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a document message from a stream.
    /// </summary>
    /// <param name="phone">Recipient phone number.</param>
    /// <param name="documentStream">
    /// Stream containing the document data. The caller retains ownership and is responsible
    /// for disposing the stream after this method returns.
    /// </param>
    /// <param name="fileName">
    /// File name displayed to recipient in WhatsApp (e.g., "report.pdf").
    /// Used to detect MIME type if mimeType parameter is null.
    /// </param>
    /// <param name="mimeType">MIME type (auto-detected from fileName extension if null).</param>
    /// <param name="caption">Optional caption.</param>
    /// <param name="quotedId">Optional message ID to reply to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing send response or error.</returns>
    /// <remarks>
    /// This method reads the entire stream content but does NOT dispose it.
    /// Wrap the stream in a using block or dispose it manually after calling this method.
    /// Peak memory usage is approximately 3.5x the file size during encoding.
    /// </remarks>
    Task<WuzResult<SendMessageResponse>> SendDocumentFromStreamAsync(
        Phone phone,
        Stream documentStream,
        string fileName,
        string? mimeType = null,
        string? caption = null,
        string? quotedId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an audio message.
    /// </summary>
    /// <param name="request">The audio message request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing send response or error.</returns>
    Task<WuzResult<SendMessageResponse>> SendAudioAsync(
        SendAudioRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a video message.
    /// </summary>
    /// <param name="request">The video message request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing send response or error.</returns>
    Task<WuzResult<SendMessageResponse>> SendVideoAsync(
        SendVideoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a sticker.
    /// </summary>
    /// <param name="request">The sticker message request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing send response or error.</returns>
    Task<WuzResult<SendMessageResponse>> SendStickerAsync(
        SendStickerRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a template message with buttons.
    /// </summary>
    /// <param name="request">The template message request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing send response or error.</returns>
    Task<WuzResult<SendMessageResponse>> SendTemplateAsync(
        SendTemplateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a location message.
    /// </summary>
    /// <param name="request">The location message request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing send response or error.</returns>
    Task<WuzResult<SendMessageResponse>> SendLocationAsync(
        SendLocationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a contact card.
    /// </summary>
    /// <param name="request">The contact message request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing send response or error.</returns>
    Task<WuzResult<SendMessageResponse>> SendContactAsync(
        SendContactRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Edits a previously sent message.
    /// </summary>
    /// <param name="request">The edit message request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing send response or error.</returns>
    Task<WuzResult<SendMessageResponse>> EditMessageAsync(
        EditMessageRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a message.
    /// </summary>
    /// <param name="request">The delete message request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult indicating success or failure.</returns>
    Task<WuzResult> DeleteMessageAsync(
        DeleteMessageRequest request,
        CancellationToken cancellationToken = default);

    // ==================== CHAT OPERATIONS ====================

    /// <summary>
    /// Marks messages as read.
    /// </summary>
    /// <param name="request">The mark as read request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult indicating success or failure.</returns>
    Task<WuzResult> MarkAsReadAsync(
        MarkAsReadRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Reacts to a message with an emoji.
    /// </summary>
    /// <param name="request">The reaction request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult indicating success or failure.</returns>
    Task<WuzResult> ReactToMessageAsync(
        ReactRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets presence state (typing, recording, etc.).
    /// </summary>
    /// <param name="request">The presence request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult indicating success or failure.</returns>
    Task<WuzResult> SetPresenceAsync(
        SetPresenceRequest request,
        CancellationToken cancellationToken = default);

    // ==================== USER INFORMATION ====================

    /// <summary>
    /// Gets information about a user.
    /// </summary>
    /// <param name="phone">The phone number to get info for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing user info or error.</returns>
    Task<WuzResult<UserInfoResponse>> GetUserInfoAsync(
        Phone phone,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if phone numbers have WhatsApp.
    /// </summary>
    /// <param name="phones">The phone numbers to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing check results or error.</returns>
    Task<WuzResult<CheckPhonesResponse>> CheckPhonesAsync(
        Phone[] phones,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user's profile picture.
    /// </summary>
    /// <param name="phone">The phone number to get avatar for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing avatar data or error.</returns>
    Task<WuzResult<AvatarResponse>> GetAvatarAsync(
        Phone phone,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the contact list.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing contacts or error.</returns>
    Task<WuzResult<ContactsResponse>> GetContactsAsync(
        CancellationToken cancellationToken = default);

    // ==================== GROUP MANAGEMENT ====================

    /// <summary>
    /// Gets list of subscribed groups.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing group list or error.</returns>
    Task<WuzResult<GroupListResponse>> GetGroupsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new group.
    /// </summary>
    /// <param name="request">The create group request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing group info or error.</returns>
    Task<WuzResult<GroupInfoResponse>> CreateGroupAsync(
        CreateGroupRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets detailed group information.
    /// </summary>
    /// <param name="groupId">The group ID (JID).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing group info or error.</returns>
    Task<WuzResult<GroupInfoResponse>> GetGroupInfoAsync(
        string groupId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets group invite link.
    /// </summary>
    /// <param name="groupId">The group ID (JID).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing invite link or error.</returns>
    Task<WuzResult<GroupInviteLinkResponse>> GetGroupInviteLinkAsync(
        string groupId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates group photo.
    /// </summary>
    /// <param name="request">The update photo request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult indicating success or failure.</returns>
    Task<WuzResult> UpdateGroupPhotoAsync(
        UpdateGroupPhotoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes group photo.
    /// </summary>
    /// <param name="groupId">The group ID (JID).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult indicating success or failure.</returns>
    Task<WuzResult> RemoveGroupPhotoAsync(
        string groupId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates group name.
    /// </summary>
    /// <param name="request">The update name request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult indicating success or failure.</returns>
    Task<WuzResult> UpdateGroupNameAsync(
        UpdateGroupNameRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates group topic/description.
    /// </summary>
    /// <param name="request">The update topic request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult indicating success or failure.</returns>
    Task<WuzResult> UpdateGroupTopicAsync(
        UpdateGroupTopicRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets group announcement-only mode.
    /// </summary>
    /// <param name="request">The set announce request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult indicating success or failure.</returns>
    Task<WuzResult> SetGroupAnnounceAsync(
        SetGroupAnnounceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Manages group participants (add/remove/promote/demote).
    /// </summary>
    /// <param name="request">The manage participants request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult indicating success or failure.</returns>
    Task<WuzResult> ManageParticipantsAsync(
        ManageParticipantsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets group lock status.
    /// </summary>
    /// <param name="request">The set lock request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult indicating success or failure.</returns>
    Task<WuzResult> SetGroupLockedAsync(
        SetGroupLockRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets disappearing messages timer.
    /// </summary>
    /// <param name="request">The set disappearing messages request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult indicating success or failure.</returns>
    Task<WuzResult> SetDisappearingMessagesAsync(
        SetDisappearingMessagesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Joins a group via invite link.
    /// </summary>
    /// <param name="request">The join group request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing group info or error.</returns>
    Task<WuzResult<GroupInfoResponse>> JoinGroupAsync(
        JoinGroupRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets information about an invite link.
    /// </summary>
    /// <param name="request">The get invite info request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing invite info or error.</returns>
    Task<WuzResult<GroupInviteInfoResponse>> GetInviteInfoAsync(
        GetInviteInfoRequest request,
        CancellationToken cancellationToken = default);

    // ==================== MEDIA DOWNLOAD ====================

    /// <summary>
    /// Downloads an image from a message.
    /// </summary>
    /// <param name="messageId">The message ID containing the image.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing media data or error.</returns>
    Task<WuzResult<MediaDownloadResponse>> DownloadImageAsync(
        string messageId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a video from a message.
    /// </summary>
    /// <param name="messageId">The message ID containing the video.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing media data or error.</returns>
    Task<WuzResult<MediaDownloadResponse>> DownloadVideoAsync(
        string messageId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads audio from a message.
    /// </summary>
    /// <param name="messageId">The message ID containing the audio.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing media data or error.</returns>
    Task<WuzResult<MediaDownloadResponse>> DownloadAudioAsync(
        string messageId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a document from a message.
    /// </summary>
    /// <param name="messageId">The message ID containing the document.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing media data or error.</returns>
    Task<WuzResult<MediaDownloadResponse>> DownloadDocumentAsync(
        string messageId,
        CancellationToken cancellationToken = default);

    // ==================== WEBHOOK CONFIGURATION ====================

    /// <summary>
    /// Sets webhook URL and events.
    /// </summary>
    /// <param name="request">The set webhook request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult indicating success or failure.</returns>
    Task<WuzResult> SetWebhookAsync(
        SetWebhookRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets current webhook configuration.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing webhook config or error.</returns>
    Task<WuzResult<WebhookConfigResponse>> GetWebhookAsync(
        CancellationToken cancellationToken = default);

    // ==================== HMAC CONFIGURATION ====================

    /// <summary>
    /// Sets HMAC signing key.
    /// </summary>
    /// <param name="request">The set HMAC key request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult indicating success or failure.</returns>
    Task<WuzResult> SetHmacKeyAsync(
        SetHmacKeyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets HMAC configuration status.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult containing HMAC config or error.</returns>
    Task<WuzResult<HmacConfigResponse>> GetHmacConfigAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes HMAC configuration.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>WuzResult indicating success or failure.</returns>
    Task<WuzResult> RemoveHmacConfigAsync(
        CancellationToken cancellationToken = default);
}
