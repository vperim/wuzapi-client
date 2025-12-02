using System;
using System.Text.RegularExpressions;
using WuzApiClient.Extensions;

namespace WuzApiClient.Models.Common;

/// <summary>
/// Represents a validated phone number in E.164 format.
/// </summary>
public readonly struct Phone : IEquatable<Phone>
{
    private static readonly Regex E164Pattern = new(
        @"^\d{10,15}$",
        RegexOptions.Compiled);

    /// <summary>
    /// Gets the phone number value (digits only, no separators).
    /// </summary>
    public string Value { get; }

    private Phone(string value)
    {
        this.Value = value;
    }

    /// <summary>
    /// Tries to create a Phone from a string value.
    /// </summary>
    /// <param name="value">The phone number string.</param>
    /// <param name="phone">The resulting Phone if valid.</param>
    /// <returns>True if the phone number is valid; otherwise, false.</returns>
    public static bool TryCreate(string? value, out Phone phone)
    {
        phone = default;

        if (value.IsNullOrWhiteSpace())
            return false;

        // Remove common separators and formatting
        var normalized = NormalizePhoneNumber(value);

        if (!E164Pattern.IsMatch(normalized))
            return false;

        phone = new Phone(normalized);
        return true;
    }

    /// <summary>
    /// Creates a Phone from a string value.
    /// </summary>
    /// <param name="value">The phone number string.</param>
    /// <returns>A valid Phone instance.</returns>
    /// <exception cref="ArgumentException">Thrown if the phone number is invalid.</exception>
    public static Phone Create(string value)
    {
        if (!TryCreate(value, out var phone))
            throw new ArgumentException($"Invalid phone number format: {value}", nameof(value));

        return phone;
    }

    /// <summary>
    /// Creates a Phone from a string value, returning null if invalid.
    /// </summary>
    /// <param name="value">The phone number string.</param>
    /// <returns>A Phone instance or null if invalid.</returns>
    public static Phone? CreateOrNull(string? value)
    {
        return TryCreate(value, out var phone) ? phone : null;
    }

    private static string NormalizePhoneNumber(string value)
    {
        // Remove common separators: +, -, spaces, parentheses, dots
        var result = new char[value.Length];
        var index = 0;

        foreach (var c in value)
        {
            if (char.IsDigit(c))
            {
                result[index++] = c;
            }
        }

        return new string(result, 0, index);
    }

    /// <summary>
    /// Converts the phone to WhatsApp JID format.
    /// </summary>
    /// <returns>The phone number as a WhatsApp JID (e.g., "5511999999999@s.whatsapp.net").</returns>
    public string ToJid() => $"{this.Value}@s.whatsapp.net";

    /// <inheritdoc/>
    public bool Equals(Phone other) => this.Value == other.Value;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Phone other && this.Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => this.Value?.GetHashCode() ?? 0;

    /// <inheritdoc/>
    public override string ToString() => this.Value;

    /// <summary>
    /// Implicitly converts a Phone to its string value.
    /// </summary>
    public static implicit operator string(Phone phone) => phone.Value;

    /// <summary>
    /// Determines whether two phones are equal.
    /// </summary>
    public static bool operator ==(Phone left, Phone right) => left.Equals(right);

    /// <summary>
    /// Determines whether two phones are not equal.
    /// </summary>
    public static bool operator !=(Phone left, Phone right) => !left.Equals(right);
}
