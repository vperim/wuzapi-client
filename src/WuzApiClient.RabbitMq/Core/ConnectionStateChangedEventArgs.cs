using System;

namespace WuzApiClient.RabbitMq.Core;

/// <summary>
/// Event arguments for connection state changes.
/// </summary>
public sealed record ConnectionStateChangedEventArgs(
    bool IsConnected,
    string? Reason = null,
    Exception? Exception = null);
