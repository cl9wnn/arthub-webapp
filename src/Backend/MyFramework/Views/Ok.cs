using System.Net;
using MyFramework.Contracts;
namespace MyFramework.Views;

public class Ok: IMyActionResult
{
    public Task ExecuteAsync(HttpListenerContext context, CancellationToken cancellationToken)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        
        context.Response.StatusCode = 200;
        return Task.CompletedTask;
    }    
}