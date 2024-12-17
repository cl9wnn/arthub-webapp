namespace ArtHub;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class RouteAttribute: Attribute
{
    public string Path { get; }
    public string Method { get; }

    public RouteAttribute(string path, string method)
    {
        Path = path;
        Method = method.ToUpper();
    }
}