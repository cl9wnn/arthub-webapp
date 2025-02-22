using System.Net;

namespace MyFramework;

public class App
{
    private readonly HttpListener _httpListener;
    private readonly RouteHandler _routeHandler;

    public App(RouteHandler routeHandler)
    {
        _httpListener = new HttpListener();
        _httpListener.Prefixes.Add("http://localhost:5050/");
        _routeHandler = routeHandler;
    }

    public async Task StartAsync()
    {
        try
        {
            _httpListener.Start();
            await Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
        finally
        {
            Stop();
        }
    }

    private async Task Run()
    {
        while (_httpListener.IsListening)
        {
            var context = await _httpListener.GetContextAsync();

            _ = Task.Run(async () =>
            {
                await _routeHandler.HandleRequest(context);
                context.Response.OutputStream.Close();
                context.Response.Close();
            });
        }
    }

    private void Stop()
    {
        if (!_httpListener.IsListening) return;
        _httpListener.Stop();
        _httpListener.Close();
    }
}