using WuzApiClient.RabbitMq.Models;

namespace WuzApiClient.RabbitMq.UnitTests.TestInfrastructure.Builders;

/// <summary>
/// Fluent builder for creating test WuzEventEnvelope instances.
/// </summary>
public sealed class TestWuzEventBuilder
{
    private string eventType = "TestEvent";
    private string userId = "test-user";
    private string instanceName = "test-instance";
    private DateTimeOffset receivedAt = DateTimeOffset.UtcNow;

    /// <summary>
    /// Sets the event type.
    /// </summary>
    /// <param name="type">The event type.</param>
    /// <returns>The builder for chaining.</returns>
    public TestWuzEventBuilder WithType(string type)
    {
        this.eventType = type;
        return this;
    }

    /// <summary>
    /// Sets the user ID.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>The builder for chaining.</returns>
    public TestWuzEventBuilder WithUserId(string userId)
    {
        this.userId = userId;
        return this;
    }

    /// <summary>
    /// Sets the instance name.
    /// </summary>
    /// <param name="instanceName">The instance name.</param>
    /// <returns>The builder for chaining.</returns>
    public TestWuzEventBuilder WithInstanceName(string instanceName)
    {
        this.instanceName = instanceName;
        return this;
    }

    /// <summary>
    /// Sets the received timestamp.
    /// </summary>
    /// <param name="receivedAt">The received timestamp.</param>
    /// <returns>The builder for chaining.</returns>
    public TestWuzEventBuilder WithReceivedAt(DateTimeOffset receivedAt)
    {
        this.receivedAt = receivedAt;
        return this;
    }

    /// <summary>
    /// Builds the test event envelope.
    /// </summary>
    /// <returns>A testable WuzEventEnvelope instance.</returns>
    public WuzEventEnvelope Build()
    {
        return new WuzEventEnvelope<TestEventData>
        {
            EventType = this.eventType,
            UserId = this.userId,
            InstanceName = this.instanceName,
            ReceivedAt = this.receivedAt,
            Event = new TestEventData(),
        };
    }
}

/// <summary>
/// Simple test event data for testing purposes.
/// </summary>
internal sealed record TestEventData
{
    // Empty record - used only for testing infrastructure
}
