using System.Collections.Concurrent;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Npgsql;
namespace MyORM;

public class QueryMapper
{
    private const string _connectionString = "Host=localhost;Port=5555;Username=postgres;Password=1029384756u;Database=arthub";
    private readonly NpgsqlConnection _connection = new(_connectionString);

    private static readonly MethodInfo GetStringMethod = typeof(DataReaderExtensions).GetMethod("GetStringOrDefault")!;
    private static readonly MethodInfo GetIntMethod = typeof(DataReaderExtensions).GetMethod("GetIntOrDefault")!;

    private static readonly ConcurrentDictionary<Type, Delegate> MapperFuncs = new();
    
    
    public  async Task<List<T>> QueryAsync<T>(FormattableString sql, CancellationToken token = default)
    {
        await _connection.OpenAsync(token);
        
        await using var command = _connection.CreateCommand();
        command.CommandText = ReplaceParameters(sql.Format);
        
        for (int i = 0; i < sql.ArgumentCount; i++)
        {
            command.Parameters.AddWithValue($"@p{i}", sql.GetArgument(i)!);
        }
        
        await using var reader = await command.ExecuteReaderAsync(token);
        
        var list = new List<T>();
        var func = (Func<IDataReader, T>)MapperFuncs.GetOrAdd(typeof(T), x => Build<T>());

        while (await reader.ReadAsync(token))
        {
            list.Add(func(reader));
        }
        return list;
    }

    private static Func<IDataReader, T> Build<T>()
    {
        var readerParam = Expression.Parameter(typeof(IDataReader));
        
        var newExp = Expression.New(typeof(T));
        var memberInit = Expression.MemberInit(newExp,
            typeof(T).GetProperties().Select(p => Expression.Bind(p, BuildReadColumnExpression(readerParam, p))));
        
        return Expression.Lambda<Func<IDataReader, T>>(memberInit, readerParam).Compile();
    }

    private static Expression BuildReadColumnExpression(Expression reader, PropertyInfo prop)
    {
        var columnName = prop.GetCustomAttribute<ColumnNameAttribute>()?.Name ?? prop.Name;
        
        if (prop.PropertyType == typeof(string))
            return Expression.Call(null, GetStringMethod, reader, Expression.Constant(columnName));
        else if (prop.PropertyType == typeof(int) || (prop.PropertyType == typeof(long)))
            return Expression.Call(null, GetIntMethod, reader, Expression.Constant(columnName));
        throw new InvalidOperationException();
    }

    private static string ReplaceParameters(string query)
    {
        var result = Regex.Replace(query, @"\{(\d+)\}", x => $"@p{x.Groups[1].Value}"); // {0} -> @p1
        return result;
    }
}
