using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WuzApiClient.RabbitMq.Core.Interfaces;
using WuzApiClient.RabbitMq.Models.Wuz;

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

    public async Task DispatchAsync(
        ReadOnlyMemory<byte> body,
        CancellationToken cancellationToken = default)
    {
        JsonDocument? document = null;

        try
        {
            var metadata = WuzEventMetadata.Parse(body);

            this.logger.LogDebug(
                "Dispatching event {EventType} from {UserId}/{InstanceName}",
                metadata.WaEventMetadata.Type,
                metadata.WuzEnvelope.UserId,
                metadata.WuzEnvelope.InstanceName);

            // Step 3: Create scoped service provider
            using var scope = this.scopeFactory.CreateScope();

            // Step 4: Registry lookup for typed dispatcher (never null, falls back to unknown)
            var dispatcher = this.registry.GetDispatcher(metadata.WaEventMetadata.Type);

            // Step 5: Delegate to typed dispatcher
            await dispatcher.DispatchAsync(
                metadata,
                scope.ServiceProvider,
                cancellationToken).ConfigureAwait(false);
        }
        catch (JsonException ex)
        {
            this.logger.LogError(ex, "JSON parsing failed");
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Unexpected error during dispatch");
        }
        finally
        {
            document?.Dispose();
        }
    }
}
