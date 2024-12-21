using System.Net;
using MyFramework;

var httpListener = new HttpListener();

var diContainer = new DiContainer();
//diContainer.Register<DbContext>(() => new DbContext());
//diContainer.Register<AuthService>(() => new AuthService(diContainer.Resolve<DbContext>()));
//diContainer.Register<AuthController>(() => new AuthController(diContainer.Resolve<AuthService>()));

var routeHandler = new RouteHandler(diContainer);
httpListener.Prefixes.Add("http://localhost:5050/");
httpListener.Start();

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



