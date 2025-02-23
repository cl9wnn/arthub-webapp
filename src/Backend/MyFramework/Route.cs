using System.Reflection;

namespace MyFramework;

public class Route
{
    public string Path { get; }
    public string Method { get; }
    public MethodInfo Action { get; }
    public List<string> ParameterNames { get; }

    public Route(string path, string method, MethodInfo action)
    {
        Path = path;
        Method = method;
        Action = action;
        ParameterNames = ExtractParameterNames(path);
    }

    private static List<string> ExtractParameterNames(string path)
    {
        var parameterNames = new List<string>();
        var segments = path.Split('/');

        foreach (var segment in segments)
        {
            if (segment.StartsWith("{") && segment.EndsWith("}"))
            {
                parameterNames.Add(segment.Trim('{', '}'));
            }
        }

        return parameterNames;
    }
}