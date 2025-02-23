using System.Collections.Concurrent;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Transactions;
using MyORM.interfaces;
using Npgsql;
using IsolationLevel = System.Data.IsolationLevel;

namespace MyORM;

public class QueryMapper(string connectionString) : IQueryMapper
{
    private readonly NpgsqlConnection _connection = new(connectionString);

    private static readonly MethodInfo GetStringMethod = typeof(DataReaderExtensions).GetMethod("GetStringOrDefault")!, 
        GetIntMethod = typeof(DataReaderExtensions).GetMethod("GetIntOrDefault")!,
        GetDateMethod = typeof(DataReaderExtensions).GetMethod("GetDateOrDefault")!,
        GetBoolMethod = typeof(DataReaderExtensions).GetMethod("GetBoolOrDefault")!;
    

    private static readonly ConcurrentDictionary<Type, Delegate> MapperFuncs = new();
    
    private static readonly Dictionary<Type, MethodInfo> TypeMethodMap = new()
    {
        { typeof(string), GetStringMethod },
        { typeof(int), GetIntMethod },
        { typeof(long), GetIntMethod },
        { typeof(DateTime), GetDateMethod },
        { typeof(bool), GetBoolMethod }
    };
    
    public  async Task<List<T>> ExecuteAndReturnListAsync<T>(FormattableString sql, CancellationToken token)
    {
        await _connection.OpenAsync(token);

        try
        {
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
        finally
        {
            await _connection.CloseAsync();
        }
    }
    public async Task ExecuteAsync(FormattableString sql, CancellationToken token)
    {
        await _connection.OpenAsync(token);
        try
        {
            await using var command = _connection.CreateCommand();
            command.CommandText = ReplaceParameters(sql.Format);

            for (var i = 0; i < sql.ArgumentCount; i++)
            {
                command.Parameters.AddWithValue($"@p{i}", sql.GetArgument(i)!);
            }

            await command.ExecuteNonQueryAsync(token);
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }
    public async Task<T?> ExecuteAndReturnAsync<T>(FormattableString sql, CancellationToken token) 
    {
        await _connection.OpenAsync(token);

        try
        {
            await using var command = _connection.CreateCommand();
            command.CommandText = ReplaceParameters(sql.Format);

            for (int i = 0; i < sql.ArgumentCount; i++)
            {
                command.Parameters.AddWithValue($"@p{i}", sql.GetArgument(i)!);
            }

            await using var reader = await command.ExecuteReaderAsync(token);

            if (await reader.ReadAsync(token))
            {
                if (typeof(T).IsPrimitive || typeof(T) == typeof(string) || typeof(T).IsValueType)
                {
                    return (T)Convert.ChangeType(reader.GetValue(0), typeof(T));
                }
                else
                {
                    var func = (Func<IDataReader, T>)MapperFuncs.GetOrAdd(typeof(T), x => Build<T>());
                    return func(reader);
                }
            }

            return default;
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }
    
    public async Task ExecuteTransactionAsync(
        Func<NpgsqlTransaction, Task> transactionalWork, 
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, 
        CancellationToken cancellationToken = default)
    {
        await _connection.OpenAsync(cancellationToken);

        await using var transaction = await _connection.BeginTransactionAsync(isolationLevel, cancellationToken);

        try
        {
            await transactionalWork(transaction);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new TransactionAbortedException();
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }
    
    public async Task ExecuteWithTransactionAsync(FormattableString sql, NpgsqlTransaction transaction, CancellationToken cancellationToken)
    {
        if (transaction.Connection == null)
        {
            throw new InvalidOperationException("Transaction must be associated with an open connection.");
        }

        await using var command = transaction.Connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = ReplaceParameters(sql.Format);

        for (var i = 0; i < sql.ArgumentCount; i++)
        {
            command.Parameters.AddWithValue($"@p{i}", sql.GetArgument(i)!);
        }

        await command.ExecuteNonQueryAsync(cancellationToken);
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

        if (!TypeMethodMap.TryGetValue(prop.PropertyType, out var method))
        {
            throw new InvalidOperationException($"Unsupported property type: {prop.PropertyType}");
        }

        return Expression.Call(null, method, reader, Expression.Constant(columnName));
    }
    private static string ReplaceParameters(string query)
    {
        var result = Regex.Replace(query, @"\{(\d+)\}", x => $"@p{x.Groups[1].Value}"); // {0} -> @p1
        return result;
    }
}
