using AwesomeAssertions;
using WuzApiClient.Extensions.Options;

namespace WuzApiClient.Extensions.UnitTests;

[Trait("Category", "Unit")]
public sealed class HumanizedTypingOptionsTests
{
    [Fact]
    public void Default_HasExpectedValues()
    {
        // Act
        var options = HumanizedTypingOptions.Default;

        // Assert
        options.BaseDelay.Should().Be(TimeSpan.FromMilliseconds(500));
        options.PerCharacterDelay.Should().Be(TimeSpan.FromMilliseconds(50));
        options.MaxDelay.Should().Be(TimeSpan.FromSeconds(8));
        options.Jitter.Should().Be(TimeSpan.FromMilliseconds(300));
        options.ShowTypingIndicator.Should().BeTrue();
    }

    [Fact]
    public void Fast_HasExpectedValues()
    {
        // Act
        var options = HumanizedTypingOptions.Fast;

        // Assert
        options.BaseDelay.Should().Be(TimeSpan.FromMilliseconds(300));
        options.PerCharacterDelay.Should().Be(TimeSpan.FromMilliseconds(30));
        options.MaxDelay.Should().Be(TimeSpan.FromSeconds(4));
        options.Jitter.Should().Be(TimeSpan.FromMilliseconds(150));
        options.ShowTypingIndicator.Should().BeTrue();
    }

    [Fact]
    public void Slow_HasExpectedValues()
    {
        // Act
        var options = HumanizedTypingOptions.Slow;

        // Assert
        options.BaseDelay.Should().Be(TimeSpan.FromMilliseconds(800));
        options.PerCharacterDelay.Should().Be(TimeSpan.FromMilliseconds(80));
        options.MaxDelay.Should().Be(TimeSpan.FromSeconds(12));
        options.Jitter.Should().Be(TimeSpan.FromMilliseconds(500));
        options.ShowTypingIndicator.Should().BeTrue();
    }

    [Fact]
    public void NewInstance_HasDefaultPropertyValues()
    {
        // Act
        var options = new HumanizedTypingOptions();

        // Assert - new instance should have same values as Default
        options.BaseDelay.Should().Be(TimeSpan.FromMilliseconds(500));
        options.PerCharacterDelay.Should().Be(TimeSpan.FromMilliseconds(50));
        options.MaxDelay.Should().Be(TimeSpan.FromSeconds(8));
        options.Jitter.Should().Be(TimeSpan.FromMilliseconds(300));
        options.ShowTypingIndicator.Should().BeTrue();
    }

    [Fact]
    public void WithInitializer_OverridesSpecifiedProperties()
    {
        // Act
        var options = new HumanizedTypingOptions
        {
            BaseDelay = TimeSpan.FromSeconds(1),
            ShowTypingIndicator = false
        };

        // Assert
        options.BaseDelay.Should().Be(TimeSpan.FromSeconds(1));
        options.ShowTypingIndicator.Should().BeFalse();
        // Other properties should remain at defaults
        options.PerCharacterDelay.Should().Be(TimeSpan.FromMilliseconds(50));
        options.MaxDelay.Should().Be(TimeSpan.FromSeconds(8));
        options.Jitter.Should().Be(TimeSpan.FromMilliseconds(300));
    }

    [Fact]
    public void StaticPresets_AreSameInstance()
    {
        // Assert - static presets should be singleton instances
        HumanizedTypingOptions.Default.Should().BeSameAs(HumanizedTypingOptions.Default);
        HumanizedTypingOptions.Fast.Should().BeSameAs(HumanizedTypingOptions.Fast);
        HumanizedTypingOptions.Slow.Should().BeSameAs(HumanizedTypingOptions.Slow);
    }
}
