using WuzApiClient.Common.Models;

namespace WuzApiClient.UnitTests.TestInfrastructure.Builders;

/// <summary>
/// Builder for creating Phone test data.
/// </summary>
public sealed class PhoneBuilder
{
    private string value = "5511999999999";

    /// <summary>
    /// Creates a new PhoneBuilder with default values.
    /// </summary>
    public static PhoneBuilder Default() => new();

    /// <summary>
    /// Sets the phone number value.
    /// </summary>
    /// <param name="value">The phone number string.</param>
    /// <returns>The builder instance for chaining.</returns>
    public PhoneBuilder WithValue(string value)
    {
        this.value = value;
        return this;
    }

    /// <summary>
    /// Builds the Phone instance.
    /// </summary>
    /// <returns>A validated Phone instance.</returns>
    public Phone Build() => Phone.Create(this.value);
}
