namespace MyFramework.Contracts;

public interface IMyServiceProvider
{
    object? GetService(Type serviceType);
}