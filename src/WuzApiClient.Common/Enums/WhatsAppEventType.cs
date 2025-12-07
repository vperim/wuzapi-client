// ReSharper disable InconsistentNaming
namespace WuzApiClient.Common.Enums;

/// <summary>
/// Event types that can be subscribed to when connecting a session.
/// Enum member names match wuzapi's expected string values exactly (case-sensitive).
/// Source: wuzapi/constants.go supportedEventTypes
/// </summary>
public enum WhatsAppEventType
{
    Unknown,

    // ==================== Special ====================

    /// <summary>Subscribe to all events.</summary>
    All,

    // ==================== Messages and Communication ====================

    /// <summary>
    /// Incoming WhatsApp message - All message types
    /// - Triggered for text, media (image/audio/video/document), sticker, contact, location, reactions, etc.
    /// - Processes media: downloads files, Base64 encoding, S3 upload (if configured)
    /// - Persists to message_history table if user's history setting > 0 (respects per-chat history limit)
    /// - Includes message metadata: ID, sender, chat, timestamp, quoted messages
    /// - Supports view-once and ephemeral messages
    /// - Most complex event with extensive processing
    /// </summary>
    Message,

    /// <summary>
    /// Message that could not be decrypted - Encryption Error
    /// - Triggered when end-to-end encryption fails
    /// - Indicates key mismatch or protocol error
    /// - Usually occurs during security updates or key rotation
    /// - Contains message ID and sender info
    /// - Does NOT persist to database
    /// - May require manual key reset or reconnection
    /// </summary>
    UndecryptableMessage,

    /// <summary>
    /// Message delivery receipt - ⚠️ CRITICAL BUG IN WUZAPI
    /// - WARNING: wuzapi has a bug where all receipts are sent as "ReadReceipt" type
    /// - Subscribing to 'Receipt' event will receive ZERO events
    /// - You MUST subscribe to 'ReadReceipt' to receive delivery/read confirmations
    /// - Use 'state' field to distinguish: "Delivered", "Read", or "ReadSelf"
    /// - Included for API completeness with wuzapi/constants.go
    /// </summary>
    Receipt,

    /// <summary>
    /// Media download retry requested - Media Failure Recovery
    /// - Triggered when media download fails and retry is needed
    /// - Contains message ID with failed media
    /// - Indicates temporary network issues or media expiration
    /// - Does NOT auto-retry (client must implement retry logic)
    /// - Does NOT persist to database
    /// </summary>
    MediaRetry,

    /// <summary>
    /// Message read receipt - Delivery and Read Confirmations
    /// - Triggered for ALL receipt types (due to wuzapi bug)
    /// - Check 'state' field to determine actual type:
    ///   - "Delivered" = message delivered to recipient
    ///   - "Read" = message read by recipient
    ///   - "ReadSelf" = message read by own account on another device
    /// - Contains message IDs, sender, chat, timestamp
    /// - Does NOT persist to database
    /// </summary>
    ReadReceipt,

    // ==================== Groups and Contacts ====================

    /// <summary>
    /// Group information updated - Group Metadata Changes
    /// - Triggered when group name, description, subject, or settings change
    /// - Contains group JID
    /// - Does NOT persist to database (webhook-only notification)
    /// - Follow up with GET /group/info API call for complete details
    /// </summary>
    GroupInfo,

    /// <summary>
    /// User joined a group - Group Membership
    /// - Triggered when authenticated user joins a new group
    /// - Can occur from: accepting invite, being added by member, or using invite link
    /// - Auto-triggered during PairSuccess if auto-sync enabled
    /// - Contains group JID
    /// - Does NOT persist to database
    /// - Client should call GET /groups to refresh group list
    /// </summary>
    JoinedGroup,

    /// <summary>
    /// Profile or group picture updated - Picture Changes
    /// - Triggered when contact profile picture or group picture changes
    /// - Applies to both individuals and groups (determined by JID format)
    /// - Contains only JID - does NOT include actual image data
    /// - Requires separate API call to GET /contacts/{jid}/picture to fetch image
    /// - Does NOT cache or store picture metadata
    /// </summary>
    Picture,

    /// <summary>
    /// Blocklist was changed - Single Contact Block/Unblock
    /// - Triggered when a single contact is blocked or unblocked
    /// - Incremental update (not complete list)
    /// - Contains JID of affected contact
    /// - Does NOT indicate whether blocked or unblocked (client must track state)
    /// - Does NOT persist to database
    /// </summary>
    BlocklistChange,

    /// <summary>
    /// Blocklist retrieved - Complete Blocklist Sync
    /// - Triggered when complete blocklist is received from WhatsApp
    /// - Occurs on initial connection/sync or when explicitly requested
    /// - Contains full list of blocked contacts (not incremental)
    /// - Does NOT persist to database (client must store)
    /// - Sent during app state synchronization
    /// </summary>
    Blocklist,

    // ==================== Connection and Session ====================

    /// <summary>
    /// Connection established successfully - Transport Layer (WebSocket).
    /// - Triggered when WebSocket connection to WhatsApp servers is established
    /// - Sends presence available
    /// - Does NOT mean the account is authenticated
    /// </summary>
    Connected,

    /// <summary>
    /// Connection lost - Transport Layer (WebSocket)
    /// - Triggered when WebSocket connection drops
    /// - Simply logs the disconnection
    /// - Does NOT kill the client or end the session
    /// - Does NOT update DB connection status
    /// </summary>
    Disconnected,

    /// <summary>
    /// Connection attempt failed - Transport Layer (WebSocket)
    /// - Triggered after 3 connection attempts fail (immediate, 5s delay, 10s delay)
    /// - Updates DB: UPDATE users SET connected=0, qrcode=''
    /// - Exits the startClient goroutine (ends client lifecycle)
    /// - Sends webhook with error details
    /// </summary>
    ConnectFailure,

    /// <summary>
    /// Keep-alive connection restored - Transport Layer (WebSocket)
    /// - Triggered when heartbeat/ping succeeds after KeepAliveTimeout
    /// - Logs restoration event
    /// - Does NOT update DB
    /// - Does NOT kill client
    /// </summary>
    KeepAliveRestored,

    /// <summary>
    /// Keep-alive timeout occurred - Transport Layer (WebSocket)
    /// - Triggered when heartbeat/ping fails
    /// - Warns of potential connection issues
    /// - Does NOT update DB
    /// - Does NOT kill client (allows recovery)
    /// </summary>
    KeepAliveTimeout,

    /// <summary>
    /// QR code expired without successful scan - QR/Transport Layer
    /// - Triggered when QR code expires (timeout controlled by whatsmeow library, typically ~60 seconds)
    /// - Clears QR code: UPDATE users SET qrcode=''
    /// - KILLS the client via killchannel
    /// - User must restart pairing process
    /// </summary>
    QRTimeout,

    /// <summary>
    /// User was logged out - Session Layer (Authentication)
    /// - Session terminated (user logged out from WhatsApp, device unlinked, etc.)
    /// - Updates DB: UPDATE users SET connected = 0
    /// - Kills the client: killchannel[mycli.userID]
    /// - Disassociates whatsapp account from WuzApi user
    /// </summary>
    LoggedOut,

    /// <summary>
    /// Client version is outdated - Transport Layer (WebSocket)
    /// - Triggered when wuzapi/whatsmeow version is too old for WhatsApp servers
    /// - Warns that update may be needed
    /// - Does NOT update DB
    /// - Does NOT kill client (allows continued operation)
    /// </summary>
    ClientOutdated,

    /// <summary>
    /// Account is temporarily banned - Session Layer (Authentication)
    /// - Triggered when WhatsApp temporarily bans the account (spam, abuse, etc.)
    /// - Logs ban event
    /// - Does NOT update DB
    /// - Does NOT kill client
    /// </summary>
    TemporaryBan,

    /// <summary>
    /// Stream error occurred - Transport Layer (WebSocket)
    /// - Triggered on WebSocket protocol errors
    /// - Logs error details
    /// - Does NOT update DB
    /// - Does NOT kill client
    /// </summary>
    StreamError,

    /// <summary>
    /// Stream was replaced by another connection - Transport Layer (WebSocket)
    /// - Triggered when another client connects with same credentials
    /// - Logs replacement (silent event)
    /// - Does NOT send webhook (returns early to avoid notification spam on multi-device usage)
    /// - Does NOT update DB
    /// - Does NOT kill client
    /// </summary>
    StreamReplaced,

    /// <summary>
    /// Phone pairing completed successfully - Session Layer (Authentication)
    /// - QR code scanned successfully
    /// - Creates WhatsApp session (authentication)
    /// - Associates account/phone number with WuzAPI user
    /// - Updates DB: UPDATE users SET jid=$1
    /// - Triggers automatic history sync if days_to_sync_history > 0 (requires DB migration)
    /// </summary>
    PairSuccess,

    /// <summary>
    /// Phone pairing failed - Session Layer (Authentication)
    /// - QR code scan or pairing process failed
    /// - Logs error details
    /// - Does NOT update DB
    /// - Does NOT kill client
    /// </summary>
    PairError,

    /// <summary>
    /// QR code generated for pairing - QR/Transport Layer
    /// - Triggered when new QR code is generated for device pairing
    /// - Stores base64-encoded QR image: UPDATE users SET qrcode=$1
    /// - User must scan within 60 seconds (see QRTimeout)
    /// - Contains QrCodeBase64 property with PNG image data
    /// </summary>
    QR,

    /// <summary>
    /// QR code scanned but multidevice not enabled on phone - Session Layer (Authentication)
    /// - User scanned QR but WhatsApp multidevice feature is disabled
    /// - User must enable multidevice in WhatsApp settings first
    /// - ⚠️ Not implemented - falls through to default handler and only gets logged
    /// </summary>
    QRScannedWithoutMultidevice,

    // ==================== Privacy and Settings ====================

    /// <summary>
    /// Privacy settings updated - Settings Layer
    /// - Triggered when user changes privacy settings (last seen, profile photo, etc.)
    /// - Contains updated privacy configuration
    /// </summary>
    PrivacySettings,

    /// <summary>
    /// Push name (display name) updated - Settings Layer
    /// - Triggered when user's display name changes
    /// - ⚠️ In wuzapi, handled together with Connected event - both send with type='Connected'
    /// - PushNameSetting events are INDISTINGUISHABLE from Connected events in webhooks
    /// - Updates DB: connected=1 (same as Connected)
    /// </summary>
    PushNameSetting,

    /// <summary>
    /// User about (status message) updated - Settings Layer
    /// - Triggered when user's status/about text changes
    /// - Contains JID and new about text
    /// </summary>
    UserAbout,

    // ==================== Synchronization and State ====================

    /// <summary>
    /// App state update - Internal Sync Action
    /// - Triggered when app state changes received (chat archived, read state, etc.)
    /// - Contains sync action values updating application state
    /// - Logged for debugging but does NOT trigger webhook by default
    /// - Does NOT persist to database
    /// - Internal sync tracking only
    /// </summary>
    AppState,

    /// <summary>
    /// App state synchronization completed - Sync Lifecycle Marker
    /// - Triggered when complete app state sync operation finishes
    /// - Specifically after critical block app state patch (WAPatchCriticalBlock) syncs
    /// - Sends "available" presence status if user has PushName configured
    /// - Does NOT trigger webhook
    /// - Does NOT persist to database
    /// - Marks readiness to receive messages
    /// </summary>
    AppStateSyncComplete,

    /// <summary>
    /// History synchronization progress - Message History Download
    /// - Triggered when WhatsApp sends historical messages from servers
    /// - Auto-triggered on connection if auto-sync configured (days_to_sync_history > 0)
    /// - Can be manually requested via GET /session/history API
    /// - ONLY sync event that persists to database (message_history table)
    /// - Processes and saves messages asynchronously in a goroutine to avoid blocking webhook delivery
    /// - Respects history limit setting (trims old messages per chat)
    /// - Contains conversations array with messages and metadata
    /// </summary>
    HistorySync,

    /// <summary>
    /// Offline synchronization completed - Catch-up Complete Signal
    /// - Triggered when all offline changes have been synced
    /// - Indicates messages/updates missed while offline are now received
    /// - Part of reconnection/sync sequence
    /// - No additional data payload (marker event only)
    /// - Does NOT persist to database
    /// </summary>
    OfflineSyncCompleted,

    /// <summary>
    /// Offline synchronization preview available - Sync Start Notification
    /// - Triggered when WhatsApp prepares preview of offline sync data
    /// - Indicates sync is about to start
    /// - Precedes actual sync data delivery
    /// - No additional data payload (notification-only)
    /// - Does NOT persist to database
    /// </summary>
    OfflineSyncPreview,

    // ==================== Calls ====================

    /// <summary>
    /// Incoming call offer received - Call Initiation
    /// - Triggered when remote user initiates a call
    /// - Contains: caller JID, call ID, call creator, group JID (if group call)
    /// - Includes remote platform and WhatsApp version
    /// - Does NOT persist to database (no call history tracking)
    /// - No call state management in wuzapi
    /// </summary>
    CallOffer,

    /// <summary>
    /// Call was accepted - Call Connection Established
    /// - Triggered when user accepts incoming call or receives acceptance
    /// - Contains: accepter JID, call ID, call creator, timestamp
    /// - Includes remote platform and version info
    /// - Does NOT persist to database
    /// - No call duration tracking
    /// </summary>
    CallAccept,

    /// <summary>
    /// Call was terminated - Call Ended
    /// - Triggered when call ends (hang up by either party)
    /// - Contains: terminating party JID, call ID, termination reason
    /// - Reason examples: "user_hangup", "timeout"
    /// - Does NOT calculate or store call duration
    /// - Does NOT persist to database
    /// </summary>
    CallTerminate,

    /// <summary>
    /// Call offer notification - Call Alert
    /// - Triggered as notification for incoming call offers
    /// - May fire for call attempts on other devices
    /// - Contains: caller JID, call ID, media type (audio/video)
    /// - Similar to CallOffer but used for notifications/alerts
    /// - Does NOT persist to database
    /// </summary>
    CallOfferNotice,

    /// <summary>
    /// Call relay latency update - Connection Quality Metric
    /// - Triggered periodically during active calls
    /// - Reports network latency for call quality monitoring
    /// - Contains: latency value (milliseconds), call ID, timestamp
    /// - Can be high-frequency event during calls
    /// - Does NOT persist to database (no QoS tracking)
    /// - Client responsible for aggregation/analysis
    /// </summary>
    CallRelayLatency,

    // ==================== Presence and Activity ====================

    /// <summary>
    /// User presence update (online/offline) - Contact Availability
    /// - Triggered when contact's online/offline status changes
    /// - Contains: contact JID, unavailable flag, last seen timestamp
    /// - Includes 'state' field: "online" or "offline" (added by wuzapi)
    /// - LastSeen may be null for privacy-conscious users
    /// - Can be high-frequency event
    /// - Does NOT persist to database (no presence history)
    /// </summary>
    Presence,

    /// <summary>
    /// Chat typing/recording indicator - Composition State
    /// - Triggered when user starts/stops typing or recording
    /// - Contains: chat JID, sender JID, state, media type
    /// - State values: "composing" (typing/recording) or "paused" (stopped)
    /// - Media values: "" (text), "audio" (voice recording), "video" (video recording)
    /// - High-frequency event during typing/recording
    /// - Does NOT persist to database
    /// - No automatic timeout or debouncing
    /// </summary>
    ChatPresence,

    // ==================== Identity ====================

    /// <summary>
    /// Contact identity changed - Security Alert (IMPORTANT)
    /// - Triggered when contact's WhatsApp encryption identity changes
    /// - Occurs when: contact reinstalls WhatsApp, new device, key regeneration, account recovery
    /// - Contains: contact JID, previous identity hash, new identity hash
    /// - SECURITY RISK: May indicate account takeover or device change
    /// - Users should verify contact identity through out-of-band channel
    /// - Does NOT persist to database
    /// - Does NOT affect message delivery
    /// - Should trigger UI notification about security change
    /// </summary>
    IdentityChange,

    // ==================== Errors ====================

    /// <summary>
    /// CAT (Client Access Token) refresh error - Session Layer (Authentication)
    /// - Triggered when Client Access Token refresh operation fails
    /// - ⚠️ Not implemented - falls through to default handler and only gets logged
    /// - May indicate authentication/session issues
    /// </summary>
    CATRefreshError,

    // ==================== Newsletter (WhatsApp Channels) ====================

    /// <summary>
    /// User joined a newsletter/channel - Channel Subscription
    /// - Triggered when user subscribes to a WhatsApp Channel (one-way broadcast)
    /// - Channels are like Telegram channels - creator broadcasts to followers
    /// - Contains: newsletter/channel JID (format: channel_id@newsletter)
    /// - Followers receive updates but cannot reply directly
    /// - Does NOT persist to database
    /// </summary>
    NewsletterJoin,

    /// <summary>
    /// User left a newsletter/channel - Channel Unsubscription
    /// - Triggered when user unsubscribes from WhatsApp Channel
    /// - Can occur from: manual unsubscribe, admin removal, blocking channel
    /// - Contains: newsletter/channel JID
    /// - Does NOT distinguish between user-initiated vs admin-initiated
    /// - Does NOT persist to database
    /// </summary>
    NewsletterLeave,

    /// <summary>
    /// Newsletter mute state changed - Channel Notifications Toggle
    /// - Triggered when user mutes/unmutes a WhatsApp Channel
    /// - Contains: newsletter JID, muted boolean (in raw event)
    /// - Muted state NOT explicitly logged (check raw event object)
    /// - Does NOT persist to database
    /// </summary>
    NewsletterMuteChange,

    /// <summary>
    /// Newsletter live update received - Channel Live Broadcast
    /// - Triggered during WhatsApp Channel live updates/broadcasts
    /// - Indicates real-time content being broadcast by channel creator
    /// - May contain video/media streaming information
    /// - Minimal logging (no JID or identifier in logs)
    /// - Rare event - channel live updates not commonly used
    /// - Does NOT persist to database
    /// </summary>
    NewsletterLiveUpdate,

    // ==================== Facebook/Meta Bridge ====================

    /// <summary>
    /// Facebook/Meta integration message - Cross-Platform Bridge
    /// - Triggered when message received from Facebook/Instagram integration
    /// - Requires WhatsApp Business account connected to Meta Business Suite
    /// - Contains: message info (chat, sender, timestamp, ID), source identifier
    /// - CRITICAL: Does NOT process media (unlike Message event)
    /// - Does NOT download attachments, images, audio, documents
    /// - Does NOT save to message_history database
    /// - Media must be retrieved from Facebook/Instagram directly
    /// - Rare - only used in enterprise/business setups
    /// </summary>
    FBMessage
}
