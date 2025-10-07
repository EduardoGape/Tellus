using Dapper;
using System;
using System.Collections.Concurrent;

namespace TellusAPI.Application.Configurations
{
    public static class DapperTypeHandlerRegistry
    {
        private static readonly ConcurrentDictionary<Type, bool> _registeredHandlers = new();

        public static void TryRegister<T>(SqlMapper.ITypeHandler handler)
        {
            var type = typeof(T);
            if (_registeredHandlers.TryAdd(type, true))
            {
                SqlMapper.AddTypeHandler(type, handler);
            }
        }
    }
}
