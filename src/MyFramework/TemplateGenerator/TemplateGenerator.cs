using System.Text.RegularExpressions;

namespace MyFramework.TemplateGenerator;

public class TemplateGenerator : ITemplateGenerator
{
    public async Task<string> Render(string template, Dictionary<string, object> data)
    {
        template = await ProcessCondition(template, data);
        template = await ProcessLoop(template, data);
        template = await ProcessTextPlaceholders(template, data);

        return data.Keys.Aggregate(template, (current, key) =>
            current.Replace($"{{{{{key}}}}}", data[key]?.ToString() ?? string.Empty));
    }

    private async Task<string> ProcessCondition(string template, Dictionary<string, object> data)
    {
        return await Task.Run(() =>
        {
            var conditionPattern = @"{{#if\s+(\w+)}}[\r\n]*\s*(.*?)\s*{{\/if}}";
            var matches = Regex.Matches(template, conditionPattern, RegexOptions.Singleline);

            foreach (Match match in matches)
            {
                var condition = match.Groups[1].Value;
                var value = match.Groups[2].Value;

                template = data.ContainsKey(condition) && data[condition]?.ToString() == "True"
                    ? template.Replace(match.Value, value)
                    : template.Replace(match.Value, string.Empty);
            }

            return template;
        });
    }

    private async Task<string> ProcessLoop(string template, Dictionary<string, object> data)
    {
        return await Task.Run(async () =>
        {
            var loopPattern = @"{{#foreach\s+(\w+)\s+in\s+(\w+)}}(.*?){{\/foreach}}";
            var matches = Regex.Matches(template, loopPattern, RegexOptions.Singleline);

            foreach (Match match in matches)
            {
                var itemName = match.Groups[1].Value;
                var collectionName = match.Groups[2].Value;
                var loopContent = match.Groups[3].Value;

                if (data.ContainsKey(collectionName) && data[collectionName] is IEnumerable<object> collection)
                {
                    var renderedItems = await Task.WhenAll(collection.Select(async item =>
                    {
                        var localData = new Dictionary<string, object>(data);

                        if (item is Dictionary<string, string> itemDict)
                        {
                            foreach (var kvp in itemDict)
                            {
                                localData[$"{itemName}.{kvp.Key}"] = kvp.Value;
                            }
                        }

                        return await Render(loopContent, localData);
                    }));

                    template = template.Replace(match.Value, string.Join(string.Empty, renderedItems));
                }
                else
                {
                    template = template.Replace(match.Value, string.Empty);
                }
            }

            return template;
        });
    }
    
    private async Task<string> ProcessTextPlaceholders(string template, Dictionary<string, object> data)
    {
        return await Task.Run(() =>
        {
            var textPattern = @"{{(?:header|paragraph)\s+([\w]+)}}";
            var matches = Regex.Matches(template, textPattern, RegexOptions.Singleline);

            foreach (Match match in matches)
            {
                var key = match.Groups[1].Value;

                if (data.TryGetValue(key, out var value))
                {
                    var content = value?.ToString() ?? string.Empty;
                    template = template.Replace(match.Value, content);
                }
                else
                {
                    template = template.Replace(match.Value, string.Empty);
                }
            }

            return template;
        });
    }
}