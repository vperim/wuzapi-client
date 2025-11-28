using WuzApiClient.Models.Responses.Chat;
using WuzApiClient.Models.Responses.Group;
using WuzApiClient.Models.Responses.Session;
using WuzApiClient.Models.Responses.User;

namespace WuzApiClient.UnitTests.TestInfrastructure.Builders;

/// <summary>
/// Factory methods for creating common API response objects for testing.
/// </summary>
public static class ResponseBuilder
{
    /// <summary>
    /// Creates a SessionStatusResponse with the specified values.
    /// </summary>
    /// <param name="connected">Whether the session is connected.</param>
    /// <param name="loggedIn">Whether the session is logged in.</param>
    /// <param name="jid">The JID of the logged-in user.</param>
    /// <returns>A configured SessionStatusResponse.</returns>
    public static SessionStatusResponse SessionStatus(
        bool connected = true,
        bool loggedIn = true,
        string? jid = "5511999999999@s.whatsapp.net")
    {
        return new SessionStatusResponse
        {
            Connected = connected,
            LoggedIn = loggedIn,
            Jid = jid
        };
    }

    /// <summary>
    /// Creates a SendMessageResponse with the specified values.
    /// </summary>
    /// <param name="id">The message ID.</param>
    /// <param name="timestamp">The message timestamp.</param>
    /// <returns>A configured SendMessageResponse.</returns>
    public static SendMessageResponse SendMessage(
        string id = "ABCD1234567890",
        long timestamp = 1700000000)
    {
        return new SendMessageResponse
        {
            Id = id,
            Timestamp = timestamp
        };
    }

    /// <summary>
    /// Creates a QrCodeResponse with the specified QR code data.
    /// </summary>
    /// <param name="qrCode">The base64-encoded QR code image.</param>
    /// <returns>A configured QrCodeResponse.</returns>
    public static QrCodeResponse QrCode(string qrCode = "data:image/png;base64,iVBORw0KGgo...")
    {
        return new QrCodeResponse
        {
            QrCode = qrCode
        };
    }

    /// <summary>
    /// Creates a UserInfoResponse with the specified values.
    /// </summary>
    /// <param name="jid">The user's JID.</param>
    /// <param name="status">The user's status.</param>
    /// <param name="pictureId">The user's profile picture ID.</param>
    /// <returns>A configured UserInfoResponse.</returns>
    public static UserInfoResponse UserInfo(
        string? jid = "5511999999999@s.whatsapp.net",
        string? status = "Available",
        string? pictureId = null)
    {
        return new UserInfoResponse
        {
            Users = new Dictionary<string, UserInfo>
            {
                [jid!] = new UserInfo
                {
                    Status = status,
                    PictureId = pictureId
                }
            }
        };
    }

    /// <summary>
    /// Creates a GroupInfoResponse with the specified values.
    /// </summary>
    /// <param name="jid">The group JID.</param>
    /// <param name="name">The group name.</param>
    /// <param name="topic">The group topic/description.</param>
    /// <param name="groupCreated">The creation timestamp (ISO 8601 format).</param>
    /// <param name="ownerJid">The group owner JID.</param>
    /// <param name="participants">The group participants.</param>
    /// <returns>A configured GroupInfoResponse.</returns>
    public static GroupInfoResponse GroupInfo(
        string? jid = "120363123456789012@g.us",
        string? name = "Test Group",
        string? topic = "Test group description",
        string? groupCreated = "2023-11-14T00:00:00Z",
        string? ownerJid = "5511999999999@s.whatsapp.net",
        GroupParticipant[]? participants = null)
    {
        return new GroupInfoResponse
        {
            Jid = jid,
            Name = name,
            Topic = topic,
            GroupCreated = groupCreated,
            OwnerJid = ownerJid,
            Participants = participants ?? []
        };
    }

    /// <summary>
    /// Creates a GroupParticipant with the specified values.
    /// </summary>
    /// <param name="jid">The participant JID.</param>
    /// <param name="isAdmin">Whether the participant is an admin.</param>
    /// <param name="isSuperAdmin">Whether the participant is a super admin.</param>
    /// <returns>A configured GroupParticipant.</returns>
    public static GroupParticipant GroupParticipant(
        string? jid = "5511999999999@s.whatsapp.net",
        bool isAdmin = false,
        bool isSuperAdmin = false)
    {
        return new GroupParticipant
        {
            Jid = jid,
            IsAdmin = isAdmin,
            IsSuperAdmin = isSuperAdmin
        };
    }
}
