using System.Net;
using ArtHub;
using ArtHub.Handlers;

var httpListener = new HttpListener();
var routeHandler = new RouteHandler();
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



