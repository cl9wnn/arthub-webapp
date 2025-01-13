using System.Net;
using System.Reflection;
using System.Text;
using MyFramework.Attributes;
using MyFramework.Contracts;
using MyFramework.TemplateGenerator;

namespace MyFramework;

public class RouteHandler
{
    private readonly List<Route> _routes = [];
    private readonly IMyServiceProvider _serviceProvider;
    private readonly IAuthService _authService;
    private readonly ErrorSender _errorSender;
    public RouteHandler(IMyServiceProvider serviceProvider, ErrorSender errorSender, IAuthService authService)
    {
        _serviceProvider = serviceProvider;
        _authService = authService;
        _errorSender = errorSender;
        
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
        var route = GetRoute(context, out var parameters);
        if (route == null)
        {
            await _errorSender.ShowErrorPageAsync(404, "Page not found", context, ctx.Token);
            return;
        }

        var controller = CreateControllerInstance(route.Action.DeclaringType!);
        
        if (controller == null)
        {
            await _errorSender.ShowErrorPageAsync(404, "Page not found", context, ctx.Token);
            return;
        }

        if (!await AuthorizeRequest(context, route, ctx.Token))
            return;

        var args = await GetActionArguments(context, route, parameters, ctx.Token);
        await ExecuteAction(route, controller, args, context, ctx.Token);
    }
    catch (Exception ex)
    {
        await _errorSender.ShowErrorPageAsync(500, "Internal server error", context, ctx.Token);
    }
}

private Route? GetRoute(HttpListenerContext context, out Dictionary<string, string> parameters)
{
    var path = context.Request.Url?.LocalPath;
    var method = context.Request.HttpMethod;
    parameters = new Dictionary<string, string>();


    if (path == null)
        return null;

    foreach (var route in _routes)
    {
        if (route.Method.Equals(method, StringComparison.OrdinalIgnoreCase) &&
            MatchRoute(path, route.Path, out parameters))
        {
            return route;
        }
    }

    return null;
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
        await _errorSender.ShowErrorJsonAsync(401, "Not authorized", context, cancellationToken);
        return false;
    }

    if (!authorizeAttribute.Roles.Contains(user.Role!))
    {
        await  _errorSender.ShowErrorJsonAsync(403, "Forbidden: Insufficient permissions", context, cancellationToken);
        return false;
    }

    context.SetItem("userId", user.UserId);
    context.SetItem("userRole", user.Role!);
    return true;
}

private async Task<object?[]> GetActionArguments(HttpListenerContext context, Route route, 
    Dictionary<string, string> parameters, CancellationToken cancellationToken)
{
    var routeParams = parameters;
    var methodParams = route.Action.GetParameters();
    var args = new object?[methodParams.Length];

    for (var i = 0; i < methodParams.Length; i++)
    {
        var parameter = methodParams[i];
        var paramName = parameter.Name;

        if (routeParams.ContainsKey(paramName!))
        {
            args[i] = Convert.ChangeType(routeParams[paramName!], parameter.ParameterType);
        }
        else if (parameter.ParameterType == typeof(CancellationToken))
        {
            args[i] = cancellationToken;
        }
        else if (parameter.ParameterType == typeof(HttpListenerContext))
        {
            args[i] = context;
        }
        else if (parameter.GetCustomAttribute<FromBodyAttribute>() != null)
        {
            args[i] = await RequestBodyReader.ReadBodyAsync(context, cancellationToken, parameter.ParameterType);
        }
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
            await _errorSender.ShowErrorPageAsync(500, "Internal server error", context, cancellationToken);
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
       

        return string.Equals(path, template);
    }
    
    private static bool MatchRoute(string path, string template, out Dictionary<string, string> parameters)
    {
        
        parameters = new Dictionary<string, string>();
        
        if (template.EndsWith("/*"))
        {
            var basePath = template[..^2];
            return path.StartsWith(basePath);
        }

        var pathSegments = path.Trim('/').Split('/');
        var templateSegments = template.Trim('/').Split('/');

        if (pathSegments.Length != templateSegments.Length)
            return false;

        for (var i = 0; i < pathSegments.Length; i++)
        {
            if (templateSegments[i].StartsWith("{") && templateSegments[i].EndsWith("}"))
            {
                var paramName = templateSegments[i].Trim('{', '}');
                parameters[paramName] = pathSegments[i];
            }
            else if (!string.Equals(pathSegments[i], templateSegments[i], StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }
}