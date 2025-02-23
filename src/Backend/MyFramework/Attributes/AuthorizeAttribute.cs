namespace MyFramework.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class AuthorizeAttribute(params string[] roles) : Attribute
{
    public string[] Roles { get; } = roles;
}