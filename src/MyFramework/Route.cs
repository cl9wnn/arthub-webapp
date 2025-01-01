using System.Reflection;

namespace MyFramework;

public class Route(string path, string method, MethodInfo action)
{
    public string Path { get; init; } = path;
    public string Method { get; init; } = method;
    public MethodInfo Action { get; init; } = action;
}