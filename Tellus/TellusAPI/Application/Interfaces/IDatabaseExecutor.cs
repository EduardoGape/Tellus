using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace TellusAPI.Application.Interfaces
{
    public interface IDatabaseExecutor
    {
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null);
        Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null);
        Task<T> QueryFirstAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null);
        Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null);
    }
}
