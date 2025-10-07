using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using TellusAPI.Application.Interfaces;

namespace TellusAPI.Application.Services
{
    public class DapperDatabaseExecutor : IDatabaseExecutor
    {
        private readonly IDbConnection _connection;

        public DapperDatabaseExecutor(IDbConnection connection)
        {
            _connection = connection;
        }

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null)
        {
            return _connection.QueryAsync<T>(sql, param, transaction);
        }

        public Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null)
        {
            return _connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction);
        }

        public Task<T> QueryFirstAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null)
        {
            return _connection.QueryFirstAsync<T>(sql, param, transaction);
        }

        public Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null)
        {
            return _connection.ExecuteAsync(sql, param, transaction);
        }
    }
}
