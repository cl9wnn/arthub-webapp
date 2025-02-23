using System.Net;
using System.Text;
using System.Text.Json;
using MyFramework.TemplateGenerator;
namespace MyFramework;

public class ErrorSender(ITemplateGenerator templateGenerator)
{
    private const string FrontendPath = "../../../../../Frontend";

    public async Task ShowErrorPageAsync(int code, string message, HttpListenerContext context, CancellationToken token)
    {
        context.Response.StatusCode = code;
        context.Response.ContentType = "text/html";
        context.Response.ContentEncoding = Encoding.UTF8;

        const string templatePath = $"{FrontendPath}/404Page/404.html";
        var notFoundTemplate = await TemplateLoader.LoadTemplateAsync(templatePath);
            
        var links = new List<Dictionary<string, string>>
        {
            new Dictionary<string, string> { { "href", "/" }, { "text", "Main page" } },
            new Dictionary<string, string> { { "href", "/register-account" }, { "text", "Register" } }
        };

        var data = new Dictionary<string, object>
        {
            { "links", links },
            { "message", message },
            {"code", code },
        };

        var renderedTemplate = await templateGenerator.Render(notFoundTemplate, data);
        
        await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(renderedTemplate), token);
    }
    
    public async Task ShowErrorJsonAsync(int code, string message, HttpListenerContext context, CancellationToken token)
    {
        context.Response.StatusCode = code;
        context.Response.ContentType = "application/json"; 
        
        var responseBody = JsonSerializer.Serialize(message,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }
        );
        var bytes = Encoding.UTF8.GetBytes(responseBody);
        await context.Response.OutputStream.WriteAsync(bytes, token);
    }
}