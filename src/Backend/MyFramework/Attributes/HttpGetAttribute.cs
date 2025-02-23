namespace MyFramework.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class HttpGetAttribute(string path) : RouteAttribute(path, "GET");