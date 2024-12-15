using System.Net;

var httpListener = new HttpListener();
httpListener.Prefixes.Add("http://localhost:5050/");
httpListener.Start();

while (httpListener.IsListening)
{
    var context = await httpListener.GetContextAsync();
    var request = context.Request;
    var response = context.Response;
    var ctx = new CancellationTokenSource();

    _ = Task.Run(async () =>
    {
        switch (request.Url?.LocalPath)
        {
            case "/" when request.HttpMethod == "GET":
                response.StatusCode = 200;
                response.ContentType = "text/html; charset=utf-8";
                var file = await File.ReadAllBytesAsync("public/index.html", ctx.Token);
                await response.OutputStream.WriteAsync(file, ctx.Token);
                break;
            default:
                await ShowResourceFile(context, ctx.Token);
                break;
        }
        
        response.OutputStream.Close();
        response.Close();
    });
}
httpListener.Stop();
httpListener.Close();


async Task ShowResourceFile(HttpListenerContext context, CancellationToken token)
{
    if (context.Request.Url?.LocalPath == null)
    {
        context.Response.StatusCode = 404;
        return; 
    }
    
    var path = context.Request.Url.LocalPath.Split('/').Last();
    var filePath = $"public/{path}";

    if (!File.Exists(filePath))
    {
        context.Response.StatusCode = 404;
        return;
    }
    
    context.Response.ContentType = path!.Split('.').Last() switch
    {
        "html" => "text/html",
        "css" => "text/css",
        "js" => "text/js",
        "svg" => "image/svg+xml",
        "ico" => "image/x-icon",
        _ => throw new ArgumentOutOfRangeException()
    };
    
    var file = await File.ReadAllBytesAsync(filePath, token);
    await context.Response.OutputStream.WriteAsync(file, token);
}