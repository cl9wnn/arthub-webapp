using System.Net;
using System.Reflection;
using MyFramework.Attributes;
using MyFramework.Contracts;
namespace MyFramework;

public class RouteHandler
{
    private readonly List<(string Path, string Method, MethodInfo Action)> _routes = new();
    private readonly Dictionary<Type, object?> _controllers = new();
    private readonly DiContainer _diContainer;
    
    public RouteHandler(DiContainer diContainer)
    {
        _diContainer = diContainer;
        RegisterRoutes();
    }

    private void RegisterRoutes()
    {
        var assembly = Assembly.Load("WebAPI");
        var controllerTypes = assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(BaseController)));
        
        foreach (var controllerType in controllerTypes)
        {
            var controllerInstance = _diContainer.Resolve(controllerType);
            _controllers[controllerType] = controllerInstance;
            
            foreach (var method in controllerType.GetMethods())
            {
                var attributes = method.GetCustomAttributes<RouteAttribute>();
                foreach (var attribute in attributes)
                {
                    _routes.Add((attribute.Path, attribute.Method, method));
                }
            }
        }
    }
  
    public async Task HandleRequest(HttpListenerContext context)
    {
        var path = context.Request.Url?.LocalPath;
        var method = context.Request.HttpMethod;
        var ctx = new CancellationTokenSource();

        if (path == null)
        {
            await WebHelper.ShowError(404, "Page not found", context, ctx.Token);
            return;
        }
      
        var route = _routes.FirstOrDefault(r =>
            r.Method.Equals(method) &&
            MatchRoute(path, r.Path));
        
        if (route == default)
        {
            await WebHelper.ShowError(404, "Page not found", context, ctx.Token);
            return;
        }

        var action = route.Action;
        var controller = _controllers[route.Action.DeclaringType!];
        
        var authorizeAttribute = action.GetCustomAttribute<AuthorizeAttribute>() ??
                                 action.DeclaringType?.GetCustomAttribute<AuthorizeAttribute>();
        
        var result = action.Invoke(controller, new object[] { context, ctx.Token });

        if (result is Task<IActionResult> asyncResult)
        {
            var actionResult = await asyncResult;
            await actionResult.ExecuteAsync(context, ctx.Token);
        }
        else if (result is IActionResult syncResult)
        {
            await syncResult.ExecuteAsync(context, ctx.Token);
        }
    }
    
    private static bool MatchRoute(string path, string template)
    {
        if (template.EndsWith("/*"))
        {
            var basePath = template.Substring(0, template.Length - 2);
            return path.StartsWith(basePath);
        }

        return string.Equals(path, template);
    }
}