using System.Collections.Concurrent;
using System.Net;

namespace MyFramework;

public static class HttpListenerContextExtensions
{
    private static readonly ConcurrentDictionary<HttpListenerContext, Dictionary<string, object>> ContextData = new();

    public static void SetItem(this HttpListenerContext context, string key, object value)
    {
        var items = ContextData.GetOrAdd(context, _ => new Dictionary<string, object>());
        items[key] = value;
    }

    public static bool TryGetItem<T>(this HttpListenerContext context, string key, out T? value)
    {
        if (ContextData.TryGetValue(context, out var items) &&
            items.TryGetValue(key, out var obj) &&
            obj is T typedValue)
        {
            value = typedValue;
            return true;
        }

        value = default;
        return false;
    }

    public static void RemoveItems(this HttpListenerContext context)
    {
        ContextData.TryRemove(context, out _);
    }
}
