namespace MyFramework.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class HttpDeleteAttribute(string path) : RouteAttribute(path, "DELETE");