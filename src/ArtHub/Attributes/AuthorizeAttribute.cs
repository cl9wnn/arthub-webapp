namespace ArtHub.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class AuthorizeAttribute(string role) : Attribute
{
    public string Role { get; } = role;
}