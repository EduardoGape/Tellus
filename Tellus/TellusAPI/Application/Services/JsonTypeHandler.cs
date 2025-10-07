using Dapper;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Text.Json;

namespace TellusAPI.Application.Services
{
    public class JsonTypeHandler<T> : SqlMapper.TypeHandler<T>
    {
        public override void SetValue(System.Data.IDbDataParameter parameter, T? value)
        {
            if (parameter is NpgsqlParameter npgsqlParameter)
            {
                npgsqlParameter.NpgsqlDbType = NpgsqlDbType.Jsonb;
                npgsqlParameter.Value = value is null
                    ? DBNull.Value
                    : JsonSerializer.Serialize(value);
            }
            else
            {
                throw new InvalidOperationException("Parameter is not NpgsqlParameter");
            }
        }

        public override T Parse(object value)
        {
            if (value is string json)
                return JsonSerializer.Deserialize<T>(json)!;

            throw new InvalidCastException($"Cannot convert {value?.GetType()} to {typeof(T)}");
        }
    }
}
