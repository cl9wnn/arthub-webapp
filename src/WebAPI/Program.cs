using System.Net;
using Microsoft.Extensions.DependencyInjection;
using MyFramework;
using Persistence;
using WebAPI.Services;

var httpListener = new HttpListener();

httpListener.Prefixes.Add("http://localhost:5050/");
httpListener.Start();

var serviceProvider = new ServiceCollection()
    .AddSingleton<DbContext>()
    .AddSingleton<AuthService>()
    .BuildServiceProvider();

var routeHandler = new RouteHandler(serviceProvider);

while (httpListener.IsListening)
{
    var context = await httpListener.GetContextAsync();

    _ = Task.Run(async () =>
    {
        await routeHandler.HandleRequest(context);
        context.Response.OutputStream.Close();
        context.Response.Close();
    });
}
httpListener.Stop();
httpListener.Close();



