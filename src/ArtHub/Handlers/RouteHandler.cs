using System.Net;
using System.Reflection;

namespace ArtHub;

public class RouteHandler
{
    private readonly Dictionary<(string Path, string Method), MethodInfo> _routes = new();
    private readonly Dictionary<Type, object?> _controllers = new();

    public RouteHandler()
    {
        RegisterRoutes();
    }

    private void RegisterRoutes()
    {
        var controllerTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && t.GetMethods().Any(m => m.GetCustomAttributes<RouteAttribute>().Any()));
        foreach (var controllerType in controllerTypes)
        {
            var controllerInstance = Activator.CreateInstance(controllerType);
            _controllers[controllerType] = controllerInstance;
            foreach (var method in controllerType.GetMethods())
            {
                var attributes = method.GetCustomAttributes<RouteAttribute>();
                foreach (var attribute in attributes)
                {
                    _routes[(attribute.Path, attribute.Method)] = method;
                }
            }
        }
    }
    
    public async Task HandleRequest(HttpListenerContext context)
    {
        var path = context.Request.Url?.LocalPath;
        var method = context.Request.HttpMethod;
        var ctx = new CancellationTokenSource();
        
        if (path == null || !_routes.TryGetValue((path, method), out var action))
        {
            await ErrorHandler.Show404(context, ctx.Token);
            return;
        }
        
        var controller = _controllers[action.DeclaringType!];
        await (Task)action.Invoke(controller, new object[] { context, ctx.Token })!;
    }
}