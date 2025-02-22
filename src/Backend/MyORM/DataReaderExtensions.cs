﻿using System.Data;

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
        if (reader.TryGetOrdinal(columnName, out int order) && !reader.IsDBNull(order))
        {
            return reader.GetString(order);
        }
        return default;
    }
    
    public static int GetIntOrDefault(this IDataReader reader, string columnName)
    {
        if (reader.TryGetOrdinal(columnName, out int order) && !reader.IsDBNull(order))
        {
            return reader.GetInt32(order);
        }
        return -1;
    }
    
    public static DateTime GetDateOrDefault(this IDataReader reader, string columnName)
    {
        if (reader.TryGetOrdinal(columnName, out int order) && !reader.IsDBNull(order))
        {
            var date = reader.GetDateTime(order); 
            return date.Date; 
        }
        return default(DateTime); 
    }
    
    public static bool GetBoolOrDefault(this IDataReader reader, string columnName)
    {
        if (reader.TryGetOrdinal(columnName, out int order) && !reader.IsDBNull(order))
        {
            return reader.GetBoolean(order);
        }
        return default(bool);
    }
}