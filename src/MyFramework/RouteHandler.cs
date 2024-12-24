using System.Net;
using System.Reflection;
using MyFramework.Attributes;
using MyFramework.Contracts;

namespace MyFramework;

public class RouteHandler
{
    private readonly List<(string Path, string Method, MethodInfo Action)> _routes = [];
    private readonly IMyServiceProvider _serviceProvider;
    private readonly IAuthService _authService;
    
    public RouteHandler(IMyServiceProvider serviceProvider, IAuthService authService)
    {
        _serviceProvider = serviceProvider;
        _authService = authService;
        RegisterRoutes();
    }

    private void RegisterRoutes()
    {
        var controllerTypes = Assembly.Load("WebAPI")
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(MyBaseController)));

        foreach (var controllerType in controllerTypes)
        {
            foreach (var method in controllerType.GetMethods((BindingFlags.Public | BindingFlags.Instance)))
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

        var controller = CreateControllerInstance(route.Action.DeclaringType!);
        
        var authorizeAttribute = route.Action.GetCustomAttribute<AuthorizeAttribute>() ??
                                 route.Action.DeclaringType?.GetCustomAttribute<AuthorizeAttribute>();
        
        if (authorizeAttribute != null)
        {
            var user = await _authService.AuthorizeUserAsync(context, ctx.Token);
            if (user == null)
            {
                await WebHelper.ShowError(401,"Not authorized", context, ctx.Token);
                return;
            }
            if (!authorizeAttribute.Role.Contains(user.Role!))
            {
                await WebHelper.ShowError(403, "Forbidden: Insufficient permissions", context, ctx.Token);
                return;
            }
        }
        
        var result = route.Action.Invoke(controller, new object[] { context, ctx.Token });
        
        if (result is Task<IMyActionResult> asyncResult)
        {
            var actionResult = await asyncResult;
            await actionResult.ExecuteAsync(context, ctx.Token);
        }
        else if (result is IMyActionResult syncResult)
        {
            await syncResult.ExecuteAsync(context, ctx.Token);
        }
    }
    
    private object? CreateControllerInstance(Type controllerType)
    {
        var constructor = controllerType.GetConstructors().FirstOrDefault();

        if (constructor == null)
            throw new InvalidOperationException($"No public constructors found for {controllerType.Name}");

        var parameters = constructor.GetParameters();
        var args = parameters.Select(p => _serviceProvider.GetService(p.ParameterType)).ToArray();

        return Activator.CreateInstance(controllerType, args);
    }
    
    private static bool MatchRoute(string path, string template)
    {
        if (template.EndsWith("/*"))
        {
            var basePath = template[..^2];
            return path.StartsWith(basePath);
        }

        return string.Equals(path, template);
    }
}