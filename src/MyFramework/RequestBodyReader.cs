using System.Net;
using System.Text;
using System.Text.Json;
using MyFramework.TemplateGenerator;

namespace MyFramework;

public static class RequestBodyReader
{

    public static async Task<T?> ReadBodyAsync<T>(HttpListenerContext context, CancellationToken cancellationToken)
    {
        using var sr = new StreamReader(context.Request.InputStream);
        var body = await sr.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        
        return JsonSerializer.Deserialize<T>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
    
    public static async Task<object?> ReadBodyAsync(HttpListenerContext context, CancellationToken cancellationToken, Type type)
    {
        using var sr = new StreamReader(context.Request.InputStream);
        var body = await sr.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        
        return JsonSerializer.Deserialize(body, type, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
 }
 