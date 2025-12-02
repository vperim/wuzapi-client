using System;
using WuzApiClient.Extensions;

namespace WuzApiClient.Models.Common;

/// <summary>
/// Represents a WhatsApp JID (Jabber ID).
/// Format: number@s.whatsapp.net for users, number@g.us for groups.
/// </summary>
public readonly struct Jid : IEquatable<Jid>
{
    private const string UserSuffix = "@s.whatsapp.net";
    private const string GroupSuffix = "@g.us";

    /// <summary>
    /// Gets the full JID value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets a value indicating whether this JID represents a user.
    /// </summary>
    public bool IsUser => this.Value?.EndsWith(UserSuffix, StringComparison.OrdinalIgnoreCase) == true;

    /// <summary>
    /// Gets a value indicating whether this JID represents a group.
    /// </summary>
    public bool IsGroup => this.Value?.EndsWith(GroupSuffix, StringComparison.OrdinalIgnoreCase) == true;

    /// <summary>
    /// Gets the number/ID portion of the JID (without the suffix).
    /// </summary>
    public string Number
    {
        get
        {
            if (string.IsNullOrEmpty(this.Value))
                return string.Empty;

            var atIndex = this.Value.IndexOf('@');
            return atIndex > 0 ? this.Value[..atIndex] : this.Value;
        }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="Jid"/>.
    /// </summary>
    /// <param name="value">The JID value.</param>
    public Jid(string value)
    {
        this.Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Creates a user JID from a phone number.
    /// </summary>
    /// <param name="phone">The phone number.</param>
    /// <returns>A user JID.</returns>
    public static Jid FromPhone(Phone phone) => new($"{phone.Value}{UserSuffix}");

    /// <summary>
    /// Creates a user JID from a phone number string.
    /// </summary>
    /// <param name="phoneNumber">The phone number string.</param>
    /// <returns>A user JID.</returns>
    public static Jid FromPhoneNumber(string phoneNumber)
    {
        var phone = Phone.Create(phoneNumber);
        return FromPhone(phone);
    }

    /// <summary>
    /// Creates a group JID from a group ID.
    /// </summary>
    /// <param name="groupId">The group ID (numbers only or full JID).</param>
    /// <returns>A group JID.</returns>
    public static Jid FromGroupId(string groupId)
    {
        if (string.IsNullOrWhiteSpace(groupId))
            throw new ArgumentException("Group ID cannot be null or empty.", nameof(groupId));

        if (groupId.EndsWith(GroupSuffix, StringComparison.OrdinalIgnoreCase))
            return new Jid(groupId);

        return new Jid($"{groupId}{GroupSuffix}");
    }

    /// <summary>
    /// Tries to parse a JID from a string value.
    /// </summary>
    /// <param name="value">The JID string.</param>
    /// <param name="jid">The resulting Jid if valid.</param>
    /// <returns>True if the JID is valid; otherwise, false.</returns>
    public static bool TryParse(string? value, out Jid jid)
    {
        jid = default;

        if (value.IsNullOrWhiteSpace())
            return false;

        if (!value.Contains("@"))
            return false;

        jid = new Jid(value);
        return true;
    }

    /// <summary>
    /// Parses a JID from a string value.
    /// </summary>
    /// <param name="value">The JID string.</param>
    /// <returns>A Jid instance.</returns>
    /// <exception cref="ArgumentException">Thrown if the JID is invalid.</exception>
    public static Jid Parse(string value)
    {
        if (!TryParse(value, out var jid))
            throw new ArgumentException($"Invalid JID format: {value}", nameof(value));

        return jid;
    }

    /// <inheritdoc/>
    public bool Equals(Jid other) =>
        string.Equals(this.Value, other.Value, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Jid other && this.Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() =>
        this.Value?.ToLowerInvariant().GetHashCode() ?? 0;

    /// <inheritdoc/>
    public override string ToString() => this.Value;

    /// <summary>
    /// Implicitly converts a Jid to its string value.
    /// </summary>
    public static implicit operator string(Jid jid) => jid.Value;

    /// <summary>
    /// Implicitly converts a string to a Jid.
    /// </summary>
    public static implicit operator Jid(string value) => new(value);

    /// <summary>
    /// Determines whether two JIDs are equal.
    /// </summary>
    public static bool operator ==(Jid left, Jid right) => left.Equals(right);

    /// <summary>
    /// Determines whether two JIDs are not equal.
    /// </summary>
    public static bool operator !=(Jid left, Jid right) => !left.Equals(right);
}
