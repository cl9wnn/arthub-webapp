namespace MyFramework.TemplateGenerator;

public interface ITemplateGenerator
{
    Task<string> Render(string template, Dictionary<string, object> data);
}
