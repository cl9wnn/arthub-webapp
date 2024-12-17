using System.Net;
using System.Reflection;
using ArtHub.Controllers;
using ArtHub.Services;

namespace ArtHub.Handlers;

public class RouteHandler
{
    private readonly Dictionary<(string Path, string Method), MethodInfo> _routes = new();
    private readonly Dictionary<Type, object?> _controllers = new();
    private readonly Dictionary<Type, object?> _dependencies = new();
    
    public RouteHandler()
    {
        RegisterDependencies();
        RegisterRoutes();
    }
    private void RegisterDependencies()
    {
        var dbContext = new DbContext();
        _dependencies[typeof(DbContext)] = dbContext;

        var authService = new AuthService(dbContext);
        _dependencies[typeof(AuthService)] = authService;

        var authController = new AuthController(authService);
        _dependencies[typeof(AuthController)] = authController;
        
        var testController = new TestController(authService);
        _dependencies[typeof(TestController)] = testController;
    }
    private void RegisterRoutes()
    {
        var controllerTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && t.GetMethods().Any(m => m.GetCustomAttributes<RouteAttribute>().Any()));
        
        foreach (var controllerType in controllerTypes)
        {
            var controllerInstance = CreateControllerInstance(controllerType);
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
    
    private object CreateControllerInstance(Type controllerType)
    {
        var constructor = controllerType.GetConstructors().FirstOrDefault();

        if (constructor == null)
            throw new InvalidOperationException($"Controller {controllerType.Name} has no public constructor.");

        var parameters = constructor.GetParameters();
        var resolvedParameters = parameters.Select<ParameterInfo, object>(p =>
        {
            if (_dependencies.TryGetValue(p.ParameterType, out var dependency))
            {
                return dependency;
            }

            throw new InvalidOperationException($"Cannot resolve dependency: {p.ParameterType.Name}");
        }).ToArray();

        return constructor.Invoke(resolvedParameters);
    }
    
    public async Task HandleRequest(HttpListenerContext context)
    {
        var path = context.Request.Url?.LocalPath;
        var method = context.Request.HttpMethod;
        var ctx = new CancellationTokenSource();
        
        if (path == null || !_routes.TryGetValue((path, method), out var action))
        {
            await WebHelper.ShowError(404,"Такой страницы нет!" , context, ctx.Token);
            return;
        }
        
        var controller = _controllers[action.DeclaringType!];
        await (Task)action.Invoke(controller, new object[] { context, ctx.Token })!;
    }
}