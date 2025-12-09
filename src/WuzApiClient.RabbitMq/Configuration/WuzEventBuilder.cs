using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using WuzApiClient.RabbitMq.Core.Interfaces;
using WuzApiClient.RabbitMq.Models;

namespace WuzApiClient.RabbitMq.Configuration;

/// <summary>
/// Fluent builder for configuring WuzApiClient.RabbitMq event handlers.
/// </summary>
public sealed class WuzEventBuilder
{
    private readonly IServiceCollection services;

    internal WuzEventBuilder(IServiceCollection services)
    {
        this.services = services ?? throw new ArgumentNullException(nameof(services));
    }

    /// <summary>
    /// Registers a typed event handler for a specific event type.
    /// </summary>
    /// <typeparam name="TEvent">The event type to handle.</typeparam>
    /// <typeparam name="THandler">The handler implementation.</typeparam>
    /// <param name="lifetime">Service lifetime (default: Scoped).</param>
    /// <returns>The builder for chaining.</returns>
    public WuzEventBuilder AddHandler<TEvent, THandler>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TEvent : class, IWhatsAppEventEnvelope
        where THandler : class, IEventHandler<TEvent>
    {
        this.services.Add(new ServiceDescriptor(
            typeof(IEventHandler<TEvent>),
            typeof(THandler),
            lifetime));

        return this;
    }

    /// <summary>
    /// Scans assemblies and registers all types implementing IEventHandler&lt;T&gt;.
    /// </summary>
    /// <param name="assemblies">Assemblies to scan for handlers.</param>
    /// <returns>The builder for chaining.</returns>
    public WuzEventBuilder AddHandlersFromAssembly(params Assembly[] assemblies)
    {
        return this.AddHandlersFromAssembly(ServiceLifetime.Scoped, assemblies);
    }

    /// <summary>
    /// Scans assemblies and registers all types implementing IEventHandler&lt;T&gt;.
    /// </summary>
    /// <param name="lifetime">Default service lifetime for discovered handlers.</param>
    /// <param name="assemblies">Assemblies to scan for handlers.</param>
    /// <returns>The builder for chaining.</returns>
    public WuzEventBuilder AddHandlersFromAssembly(ServiceLifetime lifetime, params Assembly[] assemblies)
    {
        if (assemblies == null || assemblies.Length == 0)
        {
            throw new ArgumentException("At least one assembly must be provided", nameof(assemblies));
        }

        foreach (var assembly in assemblies)
        {
            var handlerTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract);

            foreach (var handlerType in handlerTypes)
            {
                var interfaces = handlerType.GetInterfaces()
                    .Where(i => i.IsGenericType &&
                               i.GetGenericTypeDefinition() == typeof(IEventHandler<>));

                foreach (var iface in interfaces)
                {
                    this.services.Add(new ServiceDescriptor(iface, handlerType, lifetime));
                }
            }
        }

        return this;
    }
}
