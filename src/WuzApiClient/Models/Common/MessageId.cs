using System;
using WuzApiClient.Common.Extensions;

namespace WuzApiClient.Models.Common;

/// <summary>
/// Represents a WhatsApp message identifier.
/// </summary>
public readonly struct MessageId : IEquatable<MessageId>
{
    /// <summary>
    /// Gets the message ID value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="MessageId"/>.
    /// </summary>
    /// <param name="value">The message ID value.</param>
    public MessageId(string value)
    {
        this.Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Tries to create a MessageId from a string value.
    /// </summary>
    /// <param name="value">The message ID string.</param>
    /// <param name="messageId">The resulting MessageId if valid.</param>
    /// <returns>True if the message ID is valid; otherwise, false.</returns>
    public static bool TryCreate(string? value, out MessageId messageId)
    {
        messageId = default;

        if (value.IsNullOrWhiteSpace())
            return false;

        messageId = new MessageId(value);
        return true;
    }

    /// <summary>
    /// Creates a MessageId from a string value.
    /// </summary>
    /// <param name="value">The message ID string.</param>
    /// <returns>A valid MessageId instance.</returns>
    /// <exception cref="ArgumentException">Thrown if the message ID is invalid.</exception>
    public static MessageId Create(string value)
    {
        if (!TryCreate(value, out var messageId))
            throw new ArgumentException("Message ID cannot be null or empty.", nameof(value));

        return messageId;
    }

    /// <inheritdoc/>
    public bool Equals(MessageId other) => this.Value == other.Value;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is MessageId other && this.Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => this.Value?.GetHashCode() ?? 0;

    /// <inheritdoc/>
    public override string ToString() => this.Value;

    /// <summary>
    /// Implicitly converts a MessageId to its string value.
    /// </summary>
    public static implicit operator string(MessageId messageId) => messageId.Value;

    /// <summary>
    /// Implicitly converts a string to a MessageId.
    /// </summary>
    public static implicit operator MessageId(string value) => new(value);

    /// <summary>
    /// Determines whether two message IDs are equal.
    /// </summary>
    public static bool operator ==(MessageId left, MessageId right) => left.Equals(right);

    /// <summary>
    /// Determines whether two message IDs are not equal.
    /// </summary>
    public static bool operator !=(MessageId left, MessageId right) => !left.Equals(right);
}
