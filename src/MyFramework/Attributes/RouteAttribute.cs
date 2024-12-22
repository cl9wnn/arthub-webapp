namespace MyFramework.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class RouteAttribute(string path, string method) : Attribute
{
    public string Path { get; } = path;
    public string Method { get; } = method.ToUpper();
}
