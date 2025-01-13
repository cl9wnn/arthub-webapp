using System.Text;

namespace MyFramework.TemplateGenerator;

public static class TemplateLoader
{
    public static async Task<string> LoadTemplateAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Template file not found at path: {filePath}");
        }

        using var reader = new StreamReader(filePath, Encoding.UTF8);
        return await reader.ReadToEndAsync();
    }
}