namespace WuzApiClient.IntegrationTests.TestInfrastructure.Ordering;

/// <summary>
/// Attribute to mark test priority tier for ordered execution.
/// Tests are executed in tier order (0 → 1 → 2 → 3), then by order within each tier.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class TestTierAttribute : Attribute
{
    /// <summary>
    /// Gets the tier level (0 = highest priority, 3 = lowest, 99 = default).
    /// </summary>
    public int Tier { get; }

    /// <summary>
    /// Gets the order within the tier (lower runs first).
    /// </summary>
    public int Order { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestTierAttribute"/> class.
    /// </summary>
    /// <param name="tier">
    /// The tier level (0-3).
    /// Use <see cref="TestTiers"/> constants: ReadOnly=0, Messaging=1, StateModifying=2, Destructive=3.
    /// </param>
    /// <param name="order">
    /// The order within the tier (default = 0). Lower values execute first.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when tier is less than 0 or greater than 99.
    /// </exception>
    public TestTierAttribute(int tier, int order = 0)
    {
        if (tier < 0 || tier > 99)
        {
            throw new ArgumentOutOfRangeException(
                nameof(tier),
                tier,
                "Tier must be between 0 and 99. Use TestTiers constants for standard tiers.");
        }

        this.Tier = tier;
        this.Order = order;
    }
}
