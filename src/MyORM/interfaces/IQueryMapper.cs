using Npgsql;

namespace MyORM.interfaces;

public interface IQueryMapper
{
     Task<List<T>> ExecuteAndReturnListAsync<T>(FormattableString sql, CancellationToken token);
     Task ExecuteAsync(FormattableString sql, CancellationToken token);
     Task<T?> ExecuteAndReturnAsync<T>(FormattableString sql, CancellationToken token);

     Task ExecuteTransactionAsync(Func<NpgsqlTransaction, Task> transactionalWork,
         CancellationToken cancellationToken = default);
     Task ExecuteWithTransactionAsync(FormattableString sql, NpgsqlTransaction transaction,
          CancellationToken cancellationToken);
}