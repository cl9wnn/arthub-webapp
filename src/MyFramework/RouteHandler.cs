using System.Net;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text.Json;
using MyFramework.Attributes;
using MyFramework.Contracts;

namespace MyFramework;

public class RouteHandler
{
    private readonly List<Route> _routes = [];
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
                    var route = new Route(attribute.Path, attribute.Method, method);
                    _routes.Add(route);
                }
            }
        }
    }
    
  public async Task HandleRequest(HttpListenerContext context)
{
    var ctx = new CancellationTokenSource();

    try
    {
        var route = GetRoute(context);
        if (route == null)
        {
            await WebHelper.ShowError(404, "Page not found", context, ctx.Token);
            return;
        }

        var controller = CreateControllerInstance(route.Action.DeclaringType!);
        
        if (controller == null)
        {
            await WebHelper.ShowError(404, "incorrect request ", context, ctx.Token);
            return;
        }

        if (!await AuthorizeRequest(context, route, ctx.Token))
            return;

        var args = await GetActionArguments(context, route, ctx.Token);

        await ExecuteAction(route, controller, args, context, ctx.Token);
    }
    catch (Exception ex)
    {
        await WebHelper.ShowError(500, "Internal server error", context, ctx.Token);
    }
}

private Route? GetRoute(HttpListenerContext context)
{
    var path = context.Request.Url?.LocalPath;
    var method = context.Request.HttpMethod;

    if (path == null)
        return null;

    return _routes.Select(r => new Route(r.Path, r.Method, r.Action))
                  .FirstOrDefault(r =>
                      r.Method.Equals(method, StringComparison.OrdinalIgnoreCase) &&
                      MatchRoute(path, r.Path));
}

private async Task<bool> AuthorizeRequest(HttpListenerContext context, Route route, CancellationToken cancellationToken)
{
    var authorizeAttribute = route.Action.GetCustomAttribute<AuthorizeAttribute>() ??
                             route.Action.DeclaringType?.GetCustomAttribute<AuthorizeAttribute>();

    if (authorizeAttribute == null)
        return true;

    var user = await _authService.AuthorizeUserAsync(context, cancellationToken);

    if (user == null)
    {
        await WebHelper.ShowError(401, "Not authorized", context, cancellationToken);
        return false;
    }

    if (!authorizeAttribute.Roles.Contains(user.Role!))
    {
        await WebHelper.ShowError(403, "Forbidden: Insufficient permissions", context, cancellationToken);
        return false;
    }

    context.SetItem("userId", user.UserId);
    context.SetItem("userRole", user.Role!);
    return true;
}

private async Task<object?[]> GetActionArguments(HttpListenerContext context, Route route, CancellationToken cancellationToken)
{
    var parameters = route.Action.GetParameters();
    var args = new object?[parameters.Length];

    for (var i = 0; i < parameters.Length; i++)
    {
        var parameter = parameters[i];

        args[i] = parameter.ParameterType switch
        {
            Type type when parameter.GetCustomAttribute<FromBodyAttribute>() != null => await WebHelper.ReadBodyAsync(
                context, cancellationToken, parameter.ParameterType),
            Type type when type == typeof(CancellationToken) => cancellationToken,
            Type type when type == typeof(HttpListenerContext) => context,
            _ => null
        };
    }
    return args;
}

private async Task ExecuteAction(Route route, object controller, object?[] args, HttpListenerContext context, CancellationToken cancellationToken)
{
    var result = route.Action.Invoke(controller, args);

    switch (result)
    {
        case Task<IMyActionResult> asyncResult:
            var actionResult = await asyncResult;
            await actionResult.ExecuteAsync(context, cancellationToken);
            break;

        case IMyActionResult syncResult:
            await syncResult.ExecuteAsync(context, cancellationToken);
            break;

        default:
            await WebHelper.ShowError(500, "Invalid action result", context, cancellationToken);
            break;
    }
}

    
    private object? CreateControllerInstance(Type controllerType)
    {
        var constructor = controllerType.GetConstructors().FirstOrDefault();

        if (constructor == null)
            return null;

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