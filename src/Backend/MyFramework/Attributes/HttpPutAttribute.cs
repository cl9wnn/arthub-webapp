namespace MyFramework.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class HttpPutAttribute(string path) : RouteAttribute(path, "PUT");