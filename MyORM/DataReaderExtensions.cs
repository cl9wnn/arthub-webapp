using System.Data;

namespace MyORM;

public static class DataReaderExtensions
{
    private static bool TryGetOrdinal(this IDataReader reader, string columnName, out int order)
    {
        order = -1;
            
        for (int i = 0; i < reader.FieldCount; i++)
        {
            if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
            {
                order = i;
                return true;
            }
        }
        return false;
    }
            
    public static string? GetStringOrDefault(this IDataReader reader, string columnName)
    {
        if (reader.TryGetOrdinal(columnName, out int order))
        {
            return reader.GetString(order);
        }
        return default;
    }
    
    public static int GetIntOrDefault(this IDataReader reader, string columnName)
    {
        if (reader.TryGetOrdinal(columnName, out int order))
        {
            return reader.GetInt32(order);
        }
        return default;
    }
}