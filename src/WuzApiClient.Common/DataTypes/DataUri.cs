using System;

namespace WuzApiClient.Common.DataTypes;

/// <summary>
/// Represents a Data URI (RFC 2397) containing embedded data with its media type.
/// Format: data:[mediatype][;base64],data
/// </summary>
public sealed class DataUri : IEquatable<DataUri>
{
    private const string DataScheme = "data:";
    private const string Base64Marker = ";base64,";

    private readonly string value;

    /// <summary>
    /// Gets the media type (MIME type) from the Data URI.
    /// Returns "text/plain" if not specified or not parseable.
    /// </summary>
    public string MediaType { get; }

    /// <summary>
    /// Gets the raw base64-encoded data portion of the Data URI.
    /// </summary>
    public string Base64Data { get; }

    private DataUri(string value, string mediaType, string base64Data)
    {
        this.value = value;
        this.MediaType = mediaType;
        this.Base64Data = base64Data;
    }

    /// <summary>
    /// Decodes the base64 data and returns it as a byte array.
    /// </summary>
    /// <returns>The decoded bytes, or an empty array if decoding fails.</returns>
    public byte[] GetBytes()
    {
        if (string.IsNullOrEmpty(Base64Data))
            return [];

        try
        {
            return Convert.FromBase64String(Base64Data);
        }
        catch (FormatException)
        {
            return [];
        }
    }

    /// <summary>
    /// Returns the original Data URI string.
    /// </summary>
    public override string ToString() => value;

    /// <summary>
    /// Parses a Data URI string into a DataUri instance.
    /// </summary>
    /// <param name="value">The Data URI string to parse.</param>
    /// <returns>A DataUri instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when value is null.</exception>
    /// <exception cref="FormatException">Thrown when value is not a valid Data URI.</exception>
    public static DataUri Parse(string value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        if (!TryParse(value, out var result))
            throw new FormatException($"Invalid Data URI format: {value}");

        return result!;
    }

    /// <summary>
    /// Attempts to parse a Data URI string into a DataUri instance.
    /// </summary>
    /// <param name="value">The Data URI string to parse.</param>
    /// <param name="result">When successful, contains the parsed DataUri instance.</param>
    /// <returns>True if parsing succeeded; otherwise, false.</returns>
    public static bool TryParse(string? value, out DataUri? result)
    {
        result = null;

        if (string.IsNullOrEmpty(value))
            return false;

        // Must start with "data:"
        if (!value.StartsWith(DataScheme, StringComparison.OrdinalIgnoreCase))
            return false;

        // Find the base64 marker
        var base64Index = value.IndexOf(Base64Marker, StringComparison.OrdinalIgnoreCase);
        if (base64Index < 0)
            return false;

        // Extract media type (between "data:" and ";base64,")
        var mediaType = value.Substring(DataScheme.Length, base64Index - DataScheme.Length);
        if (string.IsNullOrEmpty(mediaType))
            mediaType = "text/plain";

        // Extract base64 data (after ";base64,")
        var base64Data = value.Substring(base64Index + Base64Marker.Length);

        result = new DataUri(value, mediaType, base64Data);
        return true;
    }

    /// <summary>
    /// Creates a DataUri from raw components.
    /// </summary>
    /// <param name="mediaType">The MIME type (e.g., "image/png").</param>
    /// <param name="base64Data">The base64-encoded data.</param>
    /// <returns>A new DataUri instance.</returns>
    public static DataUri Create(string mediaType, string base64Data)
    {
        if (string.IsNullOrEmpty(mediaType))
            throw new ArgumentNullException(nameof(mediaType));
        if (base64Data == null)
            throw new ArgumentNullException(nameof(base64Data));

        var value = $"{DataScheme}{mediaType}{Base64Marker}{base64Data}";
        return new DataUri(value, mediaType, base64Data);
    }

    /// <summary>
    /// Creates a DataUri from raw bytes.
    /// </summary>
    /// <param name="mediaType">The MIME type (e.g., "image/png").</param>
    /// <param name="data">The raw data bytes.</param>
    /// <returns>A new DataUri instance.</returns>
    public static DataUri Create(string mediaType, byte[] data)
    {
        if (string.IsNullOrEmpty(mediaType))
            throw new ArgumentNullException(nameof(mediaType));
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        var base64Data = Convert.ToBase64String(data);
        return Create(mediaType, base64Data);
    }

    public bool Equals(DataUri? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return string.Equals(value, other.value, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj) => Equals(obj as DataUri);

    public override int GetHashCode() => value.GetHashCode();

    public static bool operator ==(DataUri? left, DataUri? right) =>
        left?.Equals(right) ?? right is null;

    public static bool operator !=(DataUri? left, DataUri? right) => !(left == right);
}
