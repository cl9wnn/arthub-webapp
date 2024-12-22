using System.Reflection;
using MyFramework.Contracts;

namespace MyFramework;

public class MyServiceCollection: IMyServiceProvider
{
    private readonly Dictionary<Type, Func<object>> _registrations = new();

    public void AddSingleton<TService, TImplementation>() where TImplementation : TService, new()
    {
        _registrations[typeof(TService)] = () => new TImplementation();
    }
    
    public void AddSingleton<TService>()
    {
        var serviceType = typeof(TService);
        var constructor = serviceType.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
            .OrderBy(c => c.GetParameters().Length)
            .FirstOrDefault();

        if (constructor == null)
        {
            throw new InvalidOperationException($"Type {serviceType.FullName} has no public constructors.");
        }

        _registrations[serviceType] = () =>
        {
            var parameters = constructor.GetParameters()
                .Select(p => GetService(p.ParameterType))
                .ToArray();

            return constructor.Invoke(parameters);
        };
    }

    public object? GetService(Type serviceType)
    {
        if (_registrations.TryGetValue(serviceType, out var factory))
        {
            return factory();
        }
        return null;
    }
}