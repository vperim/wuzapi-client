using System.Text.Json;
using WuzApiClient.RabbitMq.Models;

namespace WuzApiClient.RabbitMq.UnitTests.TestInfrastructure.Builders;

/// <summary>
/// Fluent builder for creating test WuzEvent instances.
/// </summary>
public sealed class TestWuzEventBuilder
{
    private string type = "TestEvent";
    private string userId = "test-user";
    private string instanceName = "test-instance";
    private JsonElement? rawEvent = null;
    private DateTimeOffset receivedAt = DateTimeOffset.UtcNow;

    /// <summary>
    /// Sets the event type.
    /// </summary>
    /// <param name="type">The event type.</param>
    /// <returns>The builder for chaining.</returns>
    public TestWuzEventBuilder WithType(string type)
    {
        this.type = type;
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
    /// Sets the raw event payload.
    /// </summary>
    /// <param name="rawEvent">The raw event data.</param>
    /// <returns>The builder for chaining.</returns>
    public TestWuzEventBuilder WithRawEvent(JsonElement? rawEvent)
    {
        this.rawEvent = rawEvent;
        return this;
    }

    /// <summary>
    /// Sets the raw event payload from a JSON string.
    /// </summary>
    /// <param name="json">The JSON string.</param>
    /// <returns>The builder for chaining.</returns>
    public TestWuzEventBuilder WithRawEventJson(string json)
    {
        using var doc = JsonDocument.Parse(json);
        this.rawEvent = doc.RootElement.Clone();
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
    /// Builds the test event.
    /// </summary>
    /// <returns>A testable WuzEvent instance.</returns>
    public WuzEvent Build()
    {
        return new TestableWuzEvent
        {
            Type = this.type,
            UserId = this.userId,
            InstanceName = this.instanceName,
            RawEvent = this.rawEvent,
            ReceivedAt = this.receivedAt,
        };
    }
}

/// <summary>
/// Concrete implementation of WuzEvent for testing purposes.
/// </summary>
internal sealed record TestableWuzEvent : WuzEvent
{
    // Inherits all properties from WuzEvent base record
}
