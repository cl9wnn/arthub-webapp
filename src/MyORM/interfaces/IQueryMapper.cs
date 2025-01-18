using System.Data;
using Npgsql;

namespace MyORM.interfaces;

public interface IQueryMapper
{
    Task<List<T>> ExecuteAndReturnListAsync<T>(FormattableString sql, CancellationToken token);
    Task ExecuteAsync(FormattableString sql, CancellationToken token);
    Task<T?> ExecuteAndReturnAsync<T>(FormattableString sql, CancellationToken token);

    Task ExecuteWithTransactionAsync(FormattableString sql, NpgsqlTransaction transaction,
        CancellationToken cancellationToken);
    Task ExecuteTransactionAsync(Func<NpgsqlTransaction, Task> transactionalWork,
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);
}