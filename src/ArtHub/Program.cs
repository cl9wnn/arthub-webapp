using System.Net;
using ArtHub;

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
                await StaticFileHandler.ShowIndex(context, ctx.Token);
                break;
            default:
                await StaticFileHandler.ShowStatic(context, ctx.Token);
                break;
        }
        response.OutputStream.Close();
        response.Close();
    });
}
httpListener.Stop();
httpListener.Close();



