using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WuzApiClient.RabbitMq.Core.Interfaces;
using WuzApiClient.Results;

namespace WuzApiClient.RabbitMq.Infrastructure;

/// <summary>
/// Routes raw event bytes to typed dispatchers via registry lookup.
/// </summary>
public sealed class EventDispatcher : IEventDispatcher
{
    private readonly ITypedEventDispatcherRegistry registry;
    private readonly IServiceScopeFactory scopeFactory;
    private readonly ILogger<EventDispatcher> logger;

    public EventDispatcher(
        ITypedEventDispatcherRegistry registry,
        IServiceScopeFactory scopeFactory,
        ILogger<EventDispatcher> logger)
    {
        this.registry = registry ?? throw new ArgumentNullException(nameof(registry));
        this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<WuzResult> DispatchAsync(
        ReadOnlyMemory<byte> body,
        CancellationToken cancellationToken = default)
    {
        JsonDocument? document = null;

        try
        {
            // Step 1: Parse JSON once
            document = JsonDocument.Parse(body);
            var root = document.RootElement;

            // Step 2: Extract envelope fields
            var type = root.TryGetProperty("type", out var typeProperty)
                ? typeProperty.GetString() ?? string.Empty
                : string.Empty;

            var userId = root.TryGetProperty("userID", out var userIdProperty)
                ? userIdProperty.GetString() ?? string.Empty
                : string.Empty;

            var instanceName = root.TryGetProperty("instanceName", out var instanceProperty)
                ? instanceProperty.GetString() ?? string.Empty
                : string.Empty;

            this.logger.LogDebug(
                "Dispatching event {EventType} from {UserId}/{InstanceName}",
                type,
                userId,
                instanceName);

            // Step 3: Validate type field
            if (string.IsNullOrEmpty(type))
            {
                this.logger.LogWarning("Event missing 'type' field");
                return WuzResult.Failure(new WuzApiError(
                    WuzApiErrorCode.Unknown,
                    "Event missing 'type' field"));
            }

            // Step 4: Get event element (may be empty for some event types)
            var eventElement = root.TryGetProperty("event", out var evt)
                ? evt
                : default;

            // Step 5: Create scoped service provider
            using var scope = this.scopeFactory.CreateScope();

            // Step 6: Registry lookup for typed dispatcher (never null, falls back to unknown)
            var dispatcher = this.registry.GetDispatcher(type);

            // Step 8: Delegate to typed dispatcher
            return await dispatcher.DispatchAsync(
                eventElement,
                root,
                type,
                userId,
                instanceName,
                scope.ServiceProvider,
                cancellationToken).ConfigureAwait(false);
        }
        catch (JsonException ex)
        {
            this.logger.LogError(ex, "JSON parsing failed");
            return WuzResult.Failure(new WuzApiError(
                WuzApiErrorCode.Unknown,
                $"JSON parsing error: {ex.Message}"));
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Unexpected error during dispatch");
            return WuzResult.Failure(new WuzApiError(
                WuzApiErrorCode.Unknown,
                $"Unexpected error: {ex.Message}"));
        }
        finally
        {
            document?.Dispose();
        }
    }
}
