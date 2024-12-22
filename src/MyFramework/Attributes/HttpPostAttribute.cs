namespace MyFramework.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class HttpPostAttribute(string path) : RouteAttribute(path, "POST");