using Microsoft.Extensions.DependencyInjection;

namespace WuzApiClient.RabbitMq.UnitTests.TestInfrastructure.Mocks;

/// <summary>
/// Mock implementation of IServiceScope for testing.
/// </summary>
public sealed class MockServiceScope : IServiceScope
{
    private bool isDisposed = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="MockServiceScope"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider for this scope.</param>
    public MockServiceScope(IServiceProvider serviceProvider)
    {
        this.ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <inheritdoc/>
    public IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Gets a value indicating whether this scope has been disposed.
    /// </summary>
    public bool IsDisposed => this.isDisposed;

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!this.isDisposed)
        {
            this.isDisposed = true;
        }
    }
}

/// <summary>
/// Mock implementation of IServiceScopeFactory for testing.
/// </summary>
public sealed class MockServiceScopeFactory : IServiceScopeFactory
{
    private readonly List<MockServiceScope> createdScopes = [];
    private readonly Func<IServiceProvider>? serviceProviderFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="MockServiceScopeFactory"/> class.
    /// </summary>
    /// <param name="serviceProviderFactory">Optional factory to create service providers for each scope.</param>
    public MockServiceScopeFactory(Func<IServiceProvider>? serviceProviderFactory = null)
    {
        this.serviceProviderFactory = serviceProviderFactory;
    }

    /// <summary>
    /// Gets the list of scopes created by this factory.
    /// </summary>
    public IReadOnlyList<MockServiceScope> CreatedScopes => this.createdScopes;

    /// <summary>
    /// Gets the number of times CreateScope was called.
    /// </summary>
    public int CreateScopeCallCount { get; private set; }

    /// <inheritdoc/>
    public IServiceScope CreateScope()
    {
        this.CreateScopeCallCount++;

        IServiceProvider provider;
        if (this.serviceProviderFactory != null)
        {
            provider = this.serviceProviderFactory();
        }
        else
        {
            // Create a default empty service collection
            var services = new ServiceCollection();
            provider = services.BuildServiceProvider();
        }

        var scope = new MockServiceScope(provider);
        this.createdScopes.Add(scope);
        return scope;
    }

    /// <summary>
    /// Resets the factory, clearing all tracking information.
    /// </summary>
    public void Reset()
    {
        this.createdScopes.Clear();
        this.CreateScopeCallCount = 0;
    }
}

/// <summary>
/// Mock implementation of IServiceProvider for testing.
/// </summary>
public sealed class MockServiceProvider : IServiceProvider
{
    private readonly Dictionary<Type, object> services = new();
    private readonly Dictionary<Type, List<object>> multiServices = new();

    /// <summary>
    /// Gets the number of times GetService was called.
    /// </summary>
    public int GetServiceCallCount { get; private set; }

    /// <summary>
    /// Adds a service to the provider.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    /// <param name="instance">The service instance.</param>
    public void AddService<T>(T instance) where T : notnull
    {
        this.services[typeof(T)] = instance;
    }

    /// <summary>
    /// Adds a service to the provider by type.
    /// </summary>
    /// <param name="serviceType">The service type.</param>
    /// <param name="instance">The service instance.</param>
    public void AddService(Type serviceType, object instance)
    {
        this.services[serviceType] = instance;
    }

    /// <summary>
    /// Adds multiple services of the same type.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    /// <param name="instances">The service instances.</param>
    public void AddServices<T>(params T[] instances) where T : notnull
    {
        var type = typeof(T);
        if (!this.multiServices.ContainsKey(type))
        {
            this.multiServices[type] = [];
        }

        foreach (var instance in instances)
        {
            this.multiServices[type].Add(instance);
        }
    }

    /// <inheritdoc/>
    public object? GetService(Type serviceType)
    {
        this.GetServiceCallCount++;

        // Check if it's a request for IEnumerable<T>
        if (serviceType.IsGenericType &&
            serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            var elementType = serviceType.GetGenericArguments()[0];
            if (this.multiServices.TryGetValue(elementType, out var instances))
            {
                var listType = typeof(List<>).MakeGenericType(elementType);
                var list = Activator.CreateInstance(listType) as System.Collections.IList;
                if (list != null)
                {
                    foreach (var instance in instances)
                    {
                        list.Add(instance);
                    }

                    return list;
                }
            }

            // Return empty collection
            var emptyListType = typeof(List<>).MakeGenericType(elementType);
            return Activator.CreateInstance(emptyListType);
        }

        // Standard service lookup
        if (this.services.TryGetValue(serviceType, out var service))
        {
            return service;
        }

        return null;
    }

    /// <summary>
    /// Resets the provider, clearing all services and tracking information.
    /// </summary>
    public void Reset()
    {
        this.services.Clear();
        this.multiServices.Clear();
        this.GetServiceCallCount = 0;
    }
}
