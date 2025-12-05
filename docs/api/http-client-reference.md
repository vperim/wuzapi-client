# HTTP Client Reference

Reference for all methods available in `IWaClient` and `IWuzApiAdminClient`.

## IWaClient

Main interface for WhatsApp operations via the asternic/wuzapi gateway.

### Session Management

#### ConnectSessionAsync

Connects to WhatsApp and initiates a session.

```csharp
Task<WuzResult> ConnectSessionAsync(
    ConnectSessionRequest request,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `request` - The connection request parameters
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult` indicating success or failure

**Example:**
```csharp
using WuzApiClient.Models.Common;
using WuzApiClient.Models.Requests.Session;

// Default: subscribes to all events automatically
var connectRequest = new ConnectSessionRequest();
var result = await client.ConnectSessionAsync(connectRequest);
if (result.IsSuccess)
{
    Console.WriteLine("Session connected successfully");
}

// Or subscribe to specific events only
var selectiveRequest = new ConnectSessionRequest
{
    Subscribe = [SubscribableEvent.Message, SubscribableEvent.Receipt],
    Immediate = true
};
var selectiveResult = await client.ConnectSessionAsync(selectiveRequest);
```

---

#### DisconnectSessionAsync

Disconnects the session while preserving authentication. The session can be reconnected without scanning QR again.

```csharp
Task<WuzResult> DisconnectSessionAsync(
    CancellationToken cancellationToken = default)
```

**Returns:** `WuzResult` indicating success or failure

---

#### LogoutSessionAsync

Logs out and destroys the session completely. Will require new QR scan to reconnect.

```csharp
Task<WuzResult> LogoutSessionAsync(
    CancellationToken cancellationToken = default)
```

**Returns:** `WuzResult` indicating success or failure

---

#### GetSessionStatusAsync

Gets the current session status.

```csharp
Task<WuzResult<SessionStatusResponse>> GetSessionStatusAsync(
    CancellationToken cancellationToken = default)
```

**Returns:** `WuzResult<SessionStatusResponse>` containing session status or error

**Example:**
```csharp
var result = await client.GetSessionStatusAsync();
if (result.IsSuccess)
{
    Console.WriteLine($"Connected: {result.Value.Connected}, LoggedIn: {result.Value.LoggedIn}");
}
```

---

#### GetQrCodeAsync

Gets the QR code for authentication.

```csharp
Task<WuzResult<QrCodeResponse>> GetQrCodeAsync(
    CancellationToken cancellationToken = default)
```

**Returns:** `WuzResult<QrCodeResponse>` containing QR code data or error

**Example:**
```csharp
var result = await client.GetQrCodeAsync();
if (result.IsSuccess)
{
    var qrCode = result.Value.QrCode;
    // Display QR code to user
}
```

---

#### PairPhoneAsync

Initiates phone pairing for authentication without QR code.

```csharp
Task<WuzResult<PairPhoneResponse>> PairPhoneAsync(
    PairPhoneRequest request,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `request` - The pairing request with phone number
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult<PairPhoneResponse>` containing pairing code or error

---

#### SetProxyAsync

Configures proxy settings for the session.

```csharp
Task<WuzResult> SetProxyAsync(
    SetProxyRequest request,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `request` - The proxy configuration request
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult` indicating success or failure

---

### Messaging

#### SendTextMessageAsync

Sends a text message.

```csharp
Task<WuzResult<SendMessageResponse>> SendTextMessageAsync(
    Phone phone,
    string message,
    string? quotedId = null,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `phone` - The recipient phone number
- `message` - The message text
- `quotedId` - Optional message ID to reply to
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult<SendMessageResponse>` containing send response or error

**Example:**
```csharp
var phone = new Phone("5511999999999");
var result = await client.SendTextMessageAsync(phone, "Hello from WuzAPI!");
if (result.IsSuccess)
{
    Console.WriteLine($"Message sent: {result.Value.MessageId}");
}
```

---

#### SendImageAsync

Sends an image message.

```csharp
Task<WuzResult<SendMessageResponse>> SendImageAsync(
    SendImageRequest request,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `request` - The image message request
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult<SendMessageResponse>` containing send response or error

**Example:**
```csharp
var request = new SendImageRequest
{
    Phone = new Phone("5511999999999"),
    ImageUrl = "https://example.com/image.jpg",
    Caption = "Check out this image!"
};
var result = await client.SendImageAsync(request);
```

---

#### SendImageFromFileAsync

Sends an image message from a local file.

```csharp
Task<WuzResult<SendMessageResponse>> SendImageFromFileAsync(
    Phone phone,
    string filePath,
    string? caption = null,
    string? quotedId = null,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `phone` - The recipient phone number
- `filePath` - Path to the image file
- `caption` - Optional caption
- `quotedId` - Optional message ID to reply to
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult<SendMessageResponse>` containing send response or error

**Example:**
```csharp
var phone = new Phone("5511999999999");
var result = await client.SendImageFromFileAsync(
    phone,
    "path/to/image.jpg",
    caption: "Check this out!");

if (result.IsSuccess)
{
    Console.WriteLine($"Image sent: {result.Value.MessageId}");
}
```

**Notes:**
- MIME type is auto-detected from file extension
- File size limited to 16 MB for images
- Supported formats: JPG, PNG, GIF, WEBP

---

#### SendImageFromStreamAsync

Sends an image message from a stream.

```csharp
Task<WuzResult<SendMessageResponse>> SendImageFromStreamAsync(
    Phone phone,
    Stream imageStream,
    string? mimeType = null,
    string? caption = null,
    string? quotedId = null,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `phone` - The recipient phone number
- `imageStream` - Stream containing the image data
- `mimeType` - MIME type (e.g., "image/png"). Defaults to "image/jpeg" if null
- `caption` - Optional caption
- `quotedId` - Optional message ID to reply to
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult<SendMessageResponse>` containing send response or error

**Example:**
```csharp
var phone = new Phone("5511999999999");
using var stream = File.OpenRead("image.png");

var result = await client.SendImageFromStreamAsync(
    phone,
    stream,
    mimeType: "image/png",
    caption: "Here's the image!");

if (result.IsSuccess)
{
    Console.WriteLine($"Image sent: {result.Value.MessageId}");
}
```

**Notes:**
- Caller is responsible for disposing the stream
- Stream size limited to 16 MB for images
- For seekable streams, original position is restored after reading
- For non-seekable streams, reads from current position to EOF

---

#### SendDocumentAsync

Sends a document.

```csharp
Task<WuzResult<SendMessageResponse>> SendDocumentAsync(
    SendDocumentRequest request,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `request` - The document message request
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult<SendMessageResponse>` containing send response or error

---

#### SendDocumentFromFileAsync

Sends a document message from a local file.

```csharp
Task<WuzResult<SendMessageResponse>> SendDocumentFromFileAsync(
    Phone phone,
    string filePath,
    string? caption = null,
    string? quotedId = null,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `phone` - The recipient phone number
- `filePath` - Path to the document file
- `caption` - Optional caption
- `quotedId` - Optional message ID to reply to
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult<SendMessageResponse>` containing send response or error

**Example:**
```csharp
var phone = new Phone("5511999999999");
var result = await client.SendDocumentFromFileAsync(
    phone,
    "path/to/document.pdf",
    caption: "Here's the report");

if (result.IsSuccess)
{
    Console.WriteLine($"Document sent: {result.Value.MessageId}");
}
```

**Notes:**
- MIME type is auto-detected from file extension
- File size limited to 100 MB for documents
- Supported formats: PDF, DOC, DOCX, XLS, XLSX, PPT, PPTX, TXT, and more

---

#### SendDocumentFromStreamAsync

Sends a document message from a stream.

```csharp
Task<WuzResult<SendMessageResponse>> SendDocumentFromStreamAsync(
    Phone phone,
    Stream documentStream,
    string fileName,
    string? mimeType = null,
    string? caption = null,
    string? quotedId = null,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `phone` - The recipient phone number
- `documentStream` - Stream containing the document data
- `fileName` - File name displayed to recipient (e.g., "report.pdf"). Used to auto-detect MIME type if not provided
- `mimeType` - MIME type (auto-detected from fileName extension if null)
- `caption` - Optional caption
- `quotedId` - Optional message ID to reply to
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult<SendMessageResponse>` containing send response or error

**Example:**
```csharp
var phone = new Phone("5511999999999");
using var stream = File.OpenRead("report.pdf");

var result = await client.SendDocumentFromStreamAsync(
    phone,
    stream,
    fileName: "monthly-report.pdf",
    caption: "Monthly Report - November 2025");

if (result.IsSuccess)
{
    Console.WriteLine($"Document sent: {result.Value.MessageId}");
}
```

**Notes:**
- Caller is responsible for disposing the stream
- Stream size limited to 100 MB for documents
- For seekable streams, original position is restored after reading
- For non-seekable streams, reads from current position to EOF
- The fileName parameter is required and shown to the recipient in WhatsApp

---

#### SendAudioAsync

Sends an audio message.

```csharp
Task<WuzResult<SendMessageResponse>> SendAudioAsync(
    SendAudioRequest request,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `request` - The audio message request
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult<SendMessageResponse>` containing send response or error

---

#### SendVideoAsync

Sends a video message.

```csharp
Task<WuzResult<SendMessageResponse>> SendVideoAsync(
    SendVideoRequest request,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `request` - The video message request
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult<SendMessageResponse>` containing send response or error

---

#### SendStickerAsync

Sends a sticker.

```csharp
Task<WuzResult<SendMessageResponse>> SendStickerAsync(
    SendStickerRequest request,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `request` - The sticker message request
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult<SendMessageResponse>` containing send response or error

---

#### SendTemplateAsync

Sends a template message with buttons.

```csharp
Task<WuzResult<SendMessageResponse>> SendTemplateAsync(
    SendTemplateRequest request,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `request` - The template message request
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult<SendMessageResponse>` containing send response or error

---

#### SendLocationAsync

Sends a location message.

```csharp
Task<WuzResult<SendMessageResponse>> SendLocationAsync(
    SendLocationRequest request,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `request` - The location message request
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult<SendMessageResponse>` containing send response or error

---

#### SendContactAsync

Sends a contact card.

```csharp
Task<WuzResult<SendMessageResponse>> SendContactAsync(
    SendContactRequest request,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `request` - The contact message request
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult<SendMessageResponse>` containing send response or error

---

#### EditMessageAsync

Edits a previously sent message.

```csharp
Task<WuzResult<SendMessageResponse>> EditMessageAsync(
    EditMessageRequest request,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `request` - The edit message request
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult<SendMessageResponse>` containing send response or error

---

#### DeleteMessageAsync

Deletes a message.

```csharp
Task<WuzResult> DeleteMessageAsync(
    DeleteMessageRequest request,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `request` - The delete message request
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult` indicating success or failure

---

### Chat Operations

#### MarkAsReadAsync

Marks messages as read.

```csharp
Task<WuzResult> MarkAsReadAsync(
    MarkAsReadRequest request,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `request` - The mark as read request
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult` indicating success or failure

---

#### ReactToMessageAsync

Reacts to a message with an emoji.

```csharp
Task<WuzResult> ReactToMessageAsync(
    ReactRequest request,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `request` - The reaction request
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult` indicating success or failure

**Example:**
```csharp
var request = new ReactRequest
{
    MessageId = "MESSAGE_ID_HERE",
    Emoji = "👍"
};
var result = await client.ReactToMessageAsync(request);
```

---

#### SetPresenceAsync

Sets presence state (typing, recording, etc.).

```csharp
Task<WuzResult> SetPresenceAsync(
    SetPresenceRequest request,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `request` - The presence request
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult` indicating success or failure

---

### User Information

#### GetUserInfoAsync

Gets information about a user.

```csharp
Task<WuzResult<UserInfoResponse>> GetUserInfoAsync(
    Phone phone,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `phone` - The phone number to get info for
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult<UserInfoResponse>` containing user info or error

---

#### CheckPhonesAsync

Checks if phone numbers have WhatsApp.

```csharp
Task<WuzResult<CheckPhonesResponse>> CheckPhonesAsync(
    Phone[] phones,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `phones` - The phone numbers to check
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult<CheckPhonesResponse>` containing check results or error

**Example:**
```csharp
var phones = new[]
{
    new Phone("5511999999999"),
    new Phone("5511888888888")
};
var result = await client.CheckPhonesAsync(phones);
if (result.IsSuccess)
{
    foreach (var user in result.Value.Users)
    {
        Console.WriteLine($"{user.Query}: {user.IsInWhatsapp}");
    }
}
```

---

#### GetAvatarAsync

Gets a user's profile picture.

```csharp
Task<WuzResult<AvatarResponse>> GetAvatarAsync(
    Phone phone,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `phone` - The phone number to get avatar for
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult<AvatarResponse>` containing avatar data or error

---

#### GetContactsAsync

Gets the contact list.

```csharp
Task<WuzResult<ContactsResponse>> GetContactsAsync(
    CancellationToken cancellationToken = default)
```

**Returns:** `WuzResult<ContactsResponse>` containing contacts or error

**Example:**
```csharp
var result = await client.GetContactsAsync();
if (result.IsSuccess)
{
    foreach (var (jid, contact) in result.Value.Contacts)
    {
        Console.WriteLine($"{jid}: {contact.FullName ?? contact.PushName ?? contact.FirstName}");
    }
}
```

---

### Group Management

#### GetGroupsAsync

Gets list of subscribed groups.

```csharp
Task<WuzResult<GroupListResponse>> GetGroupsAsync(
    CancellationToken cancellationToken = default)
```

**Returns:** `WuzResult<GroupListResponse>` containing group list or error

---

#### CreateGroupAsync

Creates a new group.

```csharp
Task<WuzResult<GroupInfoResponse>> CreateGroupAsync(
    CreateGroupRequest request,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `request` - The create group request
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult<GroupInfoResponse>` containing group info or error

**Example:**
```csharp
var request = new CreateGroupRequest
{
    Name = "My Group",
    Participants = new[]
    {
        new Phone("5511999999999"),
        new Phone("5511888888888")
    }
};
var result = await client.CreateGroupAsync(request);
if (result.IsSuccess)
{
    Console.WriteLine($"Group created: {result.Value.Id}");
}
```

---

#### GetGroupInfoAsync

Gets detailed group information.

```csharp
Task<WuzResult<GroupInfoResponse>> GetGroupInfoAsync(
    string groupId,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `groupId` - The group ID (JID)
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult<GroupInfoResponse>` containing group info or error

---

#### GetGroupInviteLinkAsync

Gets group invite link.

```csharp
Task<WuzResult<GroupInviteLinkResponse>> GetGroupInviteLinkAsync(
    string groupId,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `groupId` - The group ID (JID)
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult<GroupInviteLinkResponse>` containing invite link or error

---

#### UpdateGroupPhotoAsync

Updates group photo.

```csharp
Task<WuzResult> UpdateGroupPhotoAsync(
    UpdateGroupPhotoRequest request,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `request` - The update photo request
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult` indicating success or failure

---

#### RemoveGroupPhotoAsync

Removes group photo.

```csharp
Task<WuzResult> RemoveGroupPhotoAsync(
    string groupId,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `groupId` - The group ID (JID)
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult` indicating success or failure

---

#### UpdateGroupNameAsync

Updates group name.

```csharp
Task<WuzResult> UpdateGroupNameAsync(
    UpdateGroupNameRequest request,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `request` - The update name request
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult` indicating success or failure

---

#### UpdateGroupTopicAsync

Updates group topic/description.

```csharp
Task<WuzResult> UpdateGroupTopicAsync(
    UpdateGroupTopicRequest request,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `request` - The update topic request
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult` indicating success or failure

---

#### SetGroupAnnounceAsync

Sets group announcement-only mode.

```csharp
Task<WuzResult> SetGroupAnnounceAsync(
    SetGroupAnnounceRequest request,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `request` - The set announce request
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult` indicating success or failure

---

#### ManageParticipantsAsync

Manages group participants (add/remove/promote/demote).

```csharp
Task<WuzResult> ManageParticipantsAsync(
    ManageParticipantsRequest request,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `request` - The manage participants request
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult` indicating success or failure

**Example:**
```csharp
var request = new ManageParticipantsRequest
{
    GroupId = "GROUP_ID@g.us",
    Action = ParticipantAction.Add,
    Participants = new[]
    {
        new Phone("5511999999999")
    }
};
var result = await client.ManageParticipantsAsync(request);
```

---

#### SetGroupLockedAsync

Sets group lock status.

```csharp
Task<WuzResult> SetGroupLockedAsync(
    SetGroupLockRequest request,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `request` - The set lock request
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult` indicating success or failure

---

#### SetDisappearingMessagesAsync

Sets disappearing messages timer.

```csharp
Task<WuzResult> SetDisappearingMessagesAsync(
    SetDisappearingMessagesRequest request,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `request` - The set disappearing messages request
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult` indicating success or failure

---

#### JoinGroupAsync

Joins a group via invite link.

```csharp
Task<WuzResult<GroupInfoResponse>> JoinGroupAsync(
    JoinGroupRequest request,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `request` - The join group request
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult<GroupInfoResponse>` containing group info or error

---

#### GetInviteInfoAsync

Gets information about an invite link.

```csharp
Task<WuzResult<GroupInviteInfoResponse>> GetInviteInfoAsync(
    GetInviteInfoRequest request,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `request` - The get invite info request
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult<GroupInviteInfoResponse>` containing invite info or error

---

### Media Download

#### DownloadImageAsync

Downloads an image from a message.

```csharp
Task<WuzResult<MediaDownloadResponse>> DownloadImageAsync(
    string messageId,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `messageId` - The message ID containing the image
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult<MediaDownloadResponse>` containing media data or error

**Example:**
```csharp
var result = await client.DownloadImageAsync("MESSAGE_ID_HERE");
if (result.IsSuccess)
{
    var imageData = result.Value.Data;
    var mimeType = result.Value.MimeType;
    // Process image data
}
```

---

#### DownloadVideoAsync

Downloads a video from a message.

```csharp
Task<WuzResult<MediaDownloadResponse>> DownloadVideoAsync(
    string messageId,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `messageId` - The message ID containing the video
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult<MediaDownloadResponse>` containing media data or error

---

#### DownloadAudioAsync

Downloads audio from a message.

```csharp
Task<WuzResult<MediaDownloadResponse>> DownloadAudioAsync(
    string messageId,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `messageId` - The message ID containing the audio
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult<MediaDownloadResponse>` containing media data or error

---

#### DownloadDocumentAsync

Downloads a document from a message.

```csharp
Task<WuzResult<MediaDownloadResponse>> DownloadDocumentAsync(
    string messageId,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `messageId` - The message ID containing the document
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult<MediaDownloadResponse>` containing media data or error

---

### Webhook Configuration

#### SetWebhookAsync

Sets webhook URL and events.

```csharp
Task<WuzResult> SetWebhookAsync(
    SetWebhookRequest request,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `request` - The set webhook request
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult` indicating success or failure

---

#### GetWebhookAsync

Gets current webhook configuration.

```csharp
Task<WuzResult<WebhookConfigResponse>> GetWebhookAsync(
    CancellationToken cancellationToken = default)
```

**Returns:** `WuzResult<WebhookConfigResponse>` containing webhook config or error

---

### HMAC Configuration

#### SetHmacKeyAsync

Sets HMAC signing key.

```csharp
Task<WuzResult> SetHmacKeyAsync(
    SetHmacKeyRequest request,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `request` - The set HMAC key request
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult` indicating success or failure

---

#### GetHmacConfigAsync

Gets HMAC configuration status.

```csharp
Task<WuzResult<HmacConfigResponse>> GetHmacConfigAsync(
    CancellationToken cancellationToken = default)
```

**Returns:** `WuzResult<HmacConfigResponse>` containing HMAC config or error

---

#### RemoveHmacConfigAsync

Removes HMAC configuration.

```csharp
Task<WuzResult> RemoveHmacConfigAsync(
    CancellationToken cancellationToken = default)
```

**Returns:** `WuzResult` indicating success or failure

---

## IWuzApiAdminClient

Administrative operations interface for managing WuzAPI users (instances).

### ListUsersAsync

Lists all users (instances).

```csharp
Task<WuzResult<UserListResponse>> ListUsersAsync(
    CancellationToken cancellationToken = default)
```

**Returns:** `WuzResult<UserListResponse>` containing user list or error

**Example:**
```csharp
var result = await adminClient.ListUsersAsync();
if (result.IsSuccess)
{
    foreach (var user in result.Value.Users)
    {
        Console.WriteLine($"User: {user.UserId}");
    }
}
```

---

### CreateUserAsync

Creates a new user (instance).

```csharp
Task<WuzResult<UserResponse>> CreateUserAsync(
    CreateUserRequest request,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `request` - The create user request
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult<UserResponse>` containing the created user or error

**Example:**
```csharp
var request = new CreateUserRequest
{
    UserId = "new-user-001"
};
var result = await adminClient.CreateUserAsync(request);
if (result.IsSuccess)
{
    Console.WriteLine($"User created: {result.Value.UserId}");
}
```

---

### DeleteUserAsync

Deletes a user (instance).

```csharp
Task<WuzResult> DeleteUserAsync(
    string userId,
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `userId` - The user ID to delete
- `cancellationToken` - Cancellation token

**Returns:** `WuzResult` indicating success or failure

---

## Error Handling

All methods return `WuzResult<T>` or `WuzResult`. Always check `IsSuccess` before accessing `Value`:

```csharp
var result = await client.SendTextMessageAsync(phone, message);

if (result.IsSuccess)
{
    // Use result.Value
    var messageId = result.Value.MessageId;
}
else
{
    // Handle error
    _logger.LogError("Send failed: {Error}", result.Error.Message);

    // Pattern match on error code
    switch (result.Error.Code)
    {
        case WuzApiErrorCode.SessionNotReady:
            _logger.LogWarning("Session not ready, please connect first");
            break;
        case WuzApiErrorCode.Unauthorized:
            _logger.LogError("Invalid token");
            break;
        default:
            _logger.LogError("Error code: {Code}", result.Error.Code);
            break;
    }
}
```

## Error Codes

Common `WuzApiErrorCode` values:

| Code | Value | Description |
|------|-------|-------------|
| `Unknown` | 0 | Unknown error occurred |
| `NetworkError` | 1 | Network connection error |
| `Timeout` | 2 | Request timed out |
| `DeserializationError` | 3 | Response deserialization failed |
| `BadRequest` | 400 | Bad request - invalid parameters |
| `Unauthorized` | 401 | Unauthorized - invalid token |
| `Forbidden` | 403 | Forbidden - insufficient permissions |
| `NotFound` | 404 | Resource not found |
| `Conflict` | 409 | Conflict - resource already exists |
| `RateLimitExceeded` | 429 | Rate limit exceeded |
| `InternalServerError` | 500 | Internal server error |
| `SessionNotReady` | 1000 | Session not ready (not connected/logged in) |
| `AlreadyLoggedIn` | 1001 | Session already logged in |
| `InvalidPhoneNumber` | 1002 | Invalid phone number format |
| `InvalidFile` | 1003 | Invalid file format or size |
| `InvalidRequest` | 1004 | Invalid request parameters |
| `UnexpectedResponse` | 9999 | Unexpected response from API |

## Data Types

### DataUri

RFC 2397 compliant Data URI type for embedding media data. Useful for handling base64-encoded media in messages and responses.

**Namespace:** `WuzApiClient.Common.DataTypes`

#### Structure

```csharp
public sealed class DataUri : IEquatable<DataUri>
{
    public string MediaType { get; }      // MIME type (e.g., "image/png", "application/pdf")
    public string Base64Data { get; }     // Raw base64-encoded data

    public byte[] GetBytes();             // Decode base64 to bytes
    public override string ToString();    // Returns full Data URI string
}
```

#### Creating Data URIs

**From bytes:**
```csharp
var imageBytes = await File.ReadAllBytesAsync("photo.png");
var dataUri = DataUri.Create("image/png", imageBytes);

// Result: data:image/png;base64,iVBORw0KG...
```

**From base64 string:**
```csharp
var base64 = Convert.ToBase64String(imageBytes);
var dataUri = DataUri.Create("image/jpeg", base64);
```

#### Parsing Data URIs

**Parse (throws on invalid):**
```csharp
var dataUri = DataUri.Parse("data:image/png;base64,iVBORw0KG...");
Console.WriteLine($"Media Type: {dataUri.MediaType}");
Console.WriteLine($"Data Length: {dataUri.Base64Data.Length}");
```

**TryParse (safe parsing):**
```csharp
if (DataUri.TryParse(input, out var dataUri))
{
    var bytes = dataUri.GetBytes();
    await File.WriteAllBytesAsync("output.png", bytes);
}
else
{
    Console.WriteLine("Invalid Data URI format");
}
```

#### Using with Messages

**Sending media with Data URI:**
```csharp
// Read image file
var imageBytes = await File.ReadAllBytesAsync("photo.jpg");
var dataUri = DataUri.Create("image/jpeg", imageBytes);

// Send as base64 (extract just the base64 part)
var request = new SendImageRequest
{
    Phone = new Phone("5511999999999"),
    Image = dataUri.Base64Data,  // Use Base64Data property
    MimeType = dataUri.MediaType,
    Caption = "Check this out!"
};
var result = await client.SendImageAsync(request);
```

**Processing received media:**
```csharp
// In event handler - MessageEventEnvelope with Base64 property
public async Task HandleAsync(IWuzEventEnvelope<MessageEventEnvelope> envelope, CancellationToken ct)
{
    var messageEnvelope = envelope.Payload;

    if (!string.IsNullOrEmpty(messageEnvelope.Base64))
    {
        // Create Data URI from base64 and mime type
        var dataUri = DataUri.Create(
            messageEnvelope.MimeType ?? "application/octet-stream",
            messageEnvelope.Base64
        );

        // Save to file
        var bytes = dataUri.GetBytes();
        await File.WriteAllBytesAsync($"media_{DateTime.UtcNow.Ticks}.jpg", bytes);

        _logger.LogInformation("Saved media file: {MediaType}, {Size} bytes",
            dataUri.MediaType,
            bytes.Length);
    }
}
```

#### JSON Serialization

DataUri has a custom JSON converter that automatically handles serialization:

```csharp
public class MyModel
{
    public DataUri? ProfilePicture { get; set; }
}

// Serializes to: {"profilePicture":"data:image/png;base64,iVBORw0KG..."}
var json = JsonSerializer.Serialize(new MyModel
{
    ProfilePicture = DataUri.Create("image/png", imageBytes)
});

// Deserializes back to DataUri instance
var model = JsonSerializer.Deserialize<MyModel>(json);
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `MediaType` | `string` | MIME type extracted from Data URI (defaults to "text/plain" if not specified) |
| `Base64Data` | `string` | Raw base64-encoded data portion of the Data URI |

#### Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `GetBytes()` | `byte[]` | Decodes base64 data and returns as byte array. Returns empty array if decoding fails. |
| `ToString()` | `string` | Returns the complete Data URI string in format: `data:[mediatype];base64,[data]` |
| `Parse(string)` | `DataUri` | Parses a Data URI string. Throws `FormatException` if invalid. |
| `TryParse(string, out DataUri)` | `bool` | Safely parses a Data URI string. Returns false if invalid. |
| `Create(string, string)` | `DataUri` | Creates DataUri from media type and base64 string. |
| `Create(string, byte[])` | `DataUri` | Creates DataUri from media type and raw bytes. |

#### Common Use Cases

**1. Download and convert media:**
```csharp
// Download image from message
var result = await client.DownloadImageAsync(messageId);
if (result.IsSuccess)
{
    var dataUri = DataUri.Create(
        result.Value.MimeType ?? "image/jpeg",
        result.Value.Data
    );

    // Save to file
    var bytes = dataUri.GetBytes();
    await File.WriteAllBytesAsync("download.jpg", bytes);
}
```

**2. Validate and process Data URIs:**
```csharp
public async Task<WuzResult> ProcessDataUri(string input)
{
    if (!DataUri.TryParse(input, out var dataUri))
        return WuzResult.Failure(WuzApiErrorCode.InvalidRequest, "Invalid Data URI");

    // Validate media type
    if (!dataUri.MediaType.StartsWith("image/"))
        return WuzResult.Failure(WuzApiErrorCode.InvalidFile, "Only images allowed");

    // Validate size
    var bytes = dataUri.GetBytes();
    if (bytes.Length > 5_000_000)
        return WuzResult.Failure(WuzApiErrorCode.InvalidFile, "File too large (max 5MB)");

    // Process the image...
    return WuzResult.Success();
}
```

**3. Converting between formats:**
```csharp
// Convert file to Data URI
var bytes = await File.ReadAllBytesAsync("document.pdf");
var dataUri = DataUri.Create("application/pdf", bytes);
var dataUriString = dataUri.ToString();

// Convert Data URI back to file
var parsedUri = DataUri.Parse(dataUriString);
var restoredBytes = parsedUri.GetBytes();
await File.WriteAllBytesAsync("restored.pdf", restoredBytes);
```

---

## Next Steps

- **Handle Events** → [Event Types Reference](event-types-reference.md)
- **Error Handling** → [Error Handling Guide](../usage/error-handling.md)
- **Getting Started** → [Getting Started Guide](../intro/getting-started.md)

