using Moq;
using RabbitMQ.Client;
using WuzApiClient.RabbitMq.Core.Interfaces;

namespace WuzApiClient.RabbitMq.UnitTests.TestInfrastructure.Mocks;

/// <summary>
/// Mock implementation of IRabbitMqConnection for testing.
/// Updated for RabbitMQ.Client 7.x async-first API.
/// </summary>
public sealed class MockRabbitMqConnection : IRabbitMqConnection
{
    private readonly List<string> methodCalls = [];
    private bool isDisposed = false;

    /// <summary>
    /// Gets or sets a value indicating whether the connection is connected.
    /// </summary>
    public bool IsConnected { get; set; } = true;

    /// <summary>
    /// Gets or sets the channel to return when CreateChannelAsync is called.
    /// </summary>
    public IChannel? ChannelToReturn { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether reconnection should succeed.
    /// </summary>
    public bool ReconnectSucceeds { get; set; } = true;

    /// <summary>
    /// Gets the list of method calls made to this mock.
    /// </summary>
    public IReadOnlyList<string> MethodCalls => this.methodCalls;

    /// <summary>
    /// Gets the number of times CreateChannelAsync was called.
    /// </summary>
    public int CreateChannelCallCount { get; private set; }

    /// <summary>
    /// Gets the number of times TryReconnectAsync was called.
    /// </summary>
    public int TryReconnectCallCount { get; private set; }

    /// <summary>
    /// Gets the number of times Dispose was called.
    /// </summary>
    public int DisposeCallCount { get; private set; }

    /// <summary>
    /// Gets the number of times DisposeAsync was called.
    /// </summary>
    public int DisposeAsyncCallCount { get; private set; }

    /// <inheritdoc/>
    public Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken = default)
    {
        this.ThrowIfDisposed();
        this.methodCalls.Add(nameof(this.CreateChannelAsync));
        this.CreateChannelCallCount++;

        if (this.ChannelToReturn != null)
        {
            return Task.FromResult(this.ChannelToReturn);
        }

        // Return a mock channel if none was configured
        var mockChannel = new Mock<IChannel>();
        mockChannel.Setup(c => c.IsOpen).Returns(this.IsConnected);
        return Task.FromResult(mockChannel.Object);
    }

    /// <inheritdoc/>
    public Task<bool> TryReconnectAsync(CancellationToken cancellationToken = default)
    {
        this.ThrowIfDisposed();
        this.methodCalls.Add(nameof(this.TryReconnectAsync));
        this.TryReconnectCallCount++;

        if (this.ReconnectSucceeds)
        {
            this.IsConnected = true;
        }

        return Task.FromResult(this.ReconnectSucceeds);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (this.isDisposed)
        {
            return;
        }

        this.methodCalls.Add(nameof(this.Dispose));
        this.DisposeCallCount++;
        this.isDisposed = true;
        this.IsConnected = false;
    }

    /// <inheritdoc/>
    public Task DisposeAsync()
    {
        if (this.isDisposed)
        {
            return Task.CompletedTask;
        }

        this.methodCalls.Add(nameof(this.DisposeAsync));
        this.DisposeAsyncCallCount++;
        this.isDisposed = true;
        this.IsConnected = false;

        return Task.CompletedTask;
    }

    /// <summary>
    /// Resets all tracking counters and method calls.
    /// </summary>
    public void Reset()
    {
        this.methodCalls.Clear();
        this.CreateChannelCallCount = 0;
        this.TryReconnectCallCount = 0;
        this.DisposeCallCount = 0;
        this.DisposeAsyncCallCount = 0;
        this.isDisposed = false;
        this.IsConnected = true;
    }

    private void ThrowIfDisposed()
    {
        if (this.isDisposed)
        {
            throw new ObjectDisposedException(nameof(MockRabbitMqConnection));
        }
    }
}
