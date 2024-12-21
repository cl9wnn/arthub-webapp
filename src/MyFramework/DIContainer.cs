namespace MyFramework;

public class DiContainer
{
    private readonly Dictionary<Type, Func<object>> _registrations = new();

    public void Register<TService, TImplementation>() where TImplementation : TService
    {
        _registrations[typeof(TService)] = () => Resolve(typeof(TImplementation));
    }

    public void Register<TService>(Func<object> factory)
    {
        _registrations[typeof(TService)] = factory;
    }

    public T Resolve<T>() => (T)Resolve(typeof(T));

    public object Resolve(Type serviceType)
    {
        if (_registrations.TryGetValue(serviceType, out var factory))
        {
            return factory();
        }

        var constructor = serviceType.GetConstructors().FirstOrDefault();
        if (constructor == null)
        {
            throw new InvalidOperationException($"No public constructors found for {serviceType.Name}");
        }

        var parameters = constructor.GetParameters();
        var parameterInstances = parameters.Select(p => Resolve(p.ParameterType)).ToArray();

        return Activator.CreateInstance(serviceType, parameterInstances)!;
    }
}
