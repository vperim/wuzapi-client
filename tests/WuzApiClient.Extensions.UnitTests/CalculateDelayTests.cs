using AwesomeAssertions;
using WuzApiClient.Extensions.Options;

namespace WuzApiClient.Extensions.UnitTests;

[Trait("Category", "Unit")]
public sealed class CalculateDelayTests
{
    [Fact]
    public void CalculateDelay_ZeroCharacters_ReturnsBaseDelayWithoutJitter()
    {
        // Arrange
        var options = new HumanizedTypingOptions
        {
            BaseDelay = TimeSpan.FromMilliseconds(500),
            PerCharacterDelay = TimeSpan.FromMilliseconds(50),
            MaxDelay = TimeSpan.FromSeconds(8),
            Jitter = TimeSpan.Zero
        };

        // Act
        var delay = WaClientHumanizedExtensions.CalculateDelay(0, options);

        // Assert
        delay.Should().Be(TimeSpan.FromMilliseconds(500));
    }

    [Fact]
    public void CalculateDelay_IncludesPerCharacterDelay()
    {
        // Arrange
        var options = new HumanizedTypingOptions
        {
            BaseDelay = TimeSpan.FromMilliseconds(100),
            PerCharacterDelay = TimeSpan.FromMilliseconds(10),
            MaxDelay = TimeSpan.FromSeconds(10),
            Jitter = TimeSpan.Zero
        };

        // Act
        var delay = WaClientHumanizedExtensions.CalculateDelay(50, options);

        // Assert
        // 100 + (50 * 10) = 600
        delay.Should().Be(TimeSpan.FromMilliseconds(600));
    }

    [Fact]
    public void CalculateDelay_CapsAtMaxDelay()
    {
        // Arrange
        var options = new HumanizedTypingOptions
        {
            BaseDelay = TimeSpan.FromMilliseconds(500),
            PerCharacterDelay = TimeSpan.FromMilliseconds(50),
            MaxDelay = TimeSpan.FromSeconds(2),
            Jitter = TimeSpan.Zero
        };

        // Act - 1000 chars would give 500 + 50000 = 50500ms without cap
        var delay = WaClientHumanizedExtensions.CalculateDelay(1000, options);

        // Assert - should be capped at 2000ms
        delay.Should().Be(TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void CalculateDelay_JitterAffectsDelayWithinRange()
    {
        // Arrange
        var options = new HumanizedTypingOptions
        {
            BaseDelay = TimeSpan.FromMilliseconds(500),
            PerCharacterDelay = TimeSpan.FromMilliseconds(50),
            MaxDelay = TimeSpan.FromSeconds(8),
            Jitter = TimeSpan.FromMilliseconds(100)
        };
        var baseExpected = 500 + (10 * 50); // 1000ms for 10 chars

        // Act - run multiple times to check jitter affects the result
        var delays = new TimeSpan[100];
        for (var i = 0; i < 100; i++)
        {
            delays[i] = WaClientHumanizedExtensions.CalculateDelay(10, options);
        }

        // Assert - all delays should be within jitter range
        foreach (var delay in delays)
        {
            delay.TotalMilliseconds.Should().BeGreaterThanOrEqualTo(baseExpected - 100);
            delay.TotalMilliseconds.Should().BeLessThanOrEqualTo(baseExpected + 100);
        }
    }

    [Fact]
    public void CalculateDelay_NeverReturnsNegative()
    {
        // Arrange - jitter larger than base delay
        var options = new HumanizedTypingOptions
        {
            BaseDelay = TimeSpan.FromMilliseconds(50),
            PerCharacterDelay = TimeSpan.Zero,
            MaxDelay = TimeSpan.FromSeconds(8),
            Jitter = TimeSpan.FromMilliseconds(500)
        };

        // Act - run multiple times
        for (var i = 0; i < 100; i++)
        {
            var delay = WaClientHumanizedExtensions.CalculateDelay(0, options);

            // Assert - should never be negative
            delay.TotalMilliseconds.Should().BeGreaterThanOrEqualTo(0);
        }
    }

    [Fact]
    public void CalculateDelay_DefaultOptions_ReturnsExpectedDelay()
    {
        // Arrange
        var options = HumanizedTypingOptions.Default;

        // Act - 20 chars: 500 + (20 * 50) = 1500ms (ignoring jitter)
        var delay = WaClientHumanizedExtensions.CalculateDelay(20, options);

        // Assert - within expected range with jitter
        delay.TotalMilliseconds.Should().BeGreaterThanOrEqualTo(1200); // 1500 - 300
        delay.TotalMilliseconds.Should().BeLessThanOrEqualTo(1800);    // 1500 + 300
    }

    [Fact]
    public void CalculateDelay_FastOptions_ReturnsLowerDelay()
    {
        // Arrange
        var options = HumanizedTypingOptions.Fast;

        // Act - 20 chars: 300 + (20 * 30) = 900ms (ignoring jitter)
        var delay = WaClientHumanizedExtensions.CalculateDelay(20, options);

        // Assert - within expected range with jitter (150ms)
        delay.TotalMilliseconds.Should().BeGreaterThanOrEqualTo(750);  // 900 - 150
        delay.TotalMilliseconds.Should().BeLessThanOrEqualTo(1050);   // 900 + 150
    }

    [Fact]
    public void CalculateDelay_SlowOptions_ReturnsHigherDelay()
    {
        // Arrange
        var options = HumanizedTypingOptions.Slow;

        // Act - 20 chars: 800 + (20 * 80) = 2400ms (ignoring jitter)
        var delay = WaClientHumanizedExtensions.CalculateDelay(20, options);

        // Assert - within expected range with jitter (500ms)
        delay.TotalMilliseconds.Should().BeGreaterThanOrEqualTo(1900); // 2400 - 500
        delay.TotalMilliseconds.Should().BeLessThanOrEqualTo(2900);    // 2400 + 500
    }
}
