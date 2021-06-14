using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Core.EF.Queries.Sql
{
    public static class EfExtensions
    {
        /// <summary>
        /// Creates an initial DbCommand object based on a stored procedure name
        /// </summary>
        /// <param name="context">target database context</param>
        /// <param name="storedProcName">target procedure name</param>
        /// <param name="prependDefaultSchema">Prepend the default schema name to <paramref name="storedProcName"/> if explicitly defined in <paramref name="context"/></param>
        /// <returns></returns>
        public static DbCommand LoadStoredProcedure(this DbContext context, string storedProcName, bool prependDefaultSchema = true)
        {
            var cmd = context.Database.GetDbConnection().CreateCommand();
            if (prependDefaultSchema)
            {
                var schemaName = context.Model.GetDefaultSchema();
                if (schemaName != null)
                {
                    storedProcName = $"{schemaName}.{storedProcName}";
                }
            }
            cmd.CommandText = storedProcName;
            cmd.CommandType = CommandType.StoredProcedure;
            return cmd;
        }

        /// <summary>
        /// Creates a DbParameter object and adds it to a DbCommand
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <param name="configureParam"></param>
        /// <returns></returns>
        public static DbCommand WithSqlParam(this DbCommand cmd, string paramName, object paramValue, Action<DbParameter> configureParam = null)
        {
            if (string.IsNullOrEmpty(cmd.CommandText) && cmd.CommandType != CommandType.StoredProcedure)
                throw new InvalidOperationException("Call LoadStoredProcedure before using this method");

            var param = cmd.CreateParameter();
            param.ParameterName = paramName;
            param.Value = paramValue;
            configureParam?.Invoke(param);
            cmd.Parameters.Add(param);
            return cmd;
        }

        public static DbCommand WithSqlParam(this DbCommand cmd, string paramName, Action<DbParameter> configureParam = null)
        {
            if (string.IsNullOrEmpty(cmd.CommandText) && cmd.CommandType != CommandType.StoredProcedure)
                throw new InvalidOperationException("Call LoadStoredProcedure before using this method");

            var param = cmd.CreateParameter();
            param.ParameterName = paramName;
            configureParam?.Invoke(param);
            cmd.Parameters.Add(param);
            return cmd;
        }

        public class StoredProcedureResults
        {
            private readonly DbDataReader _reader;

            public StoredProcedureResults(DbDataReader reader)
            {
                _reader = reader;
            }

            public IList<T> ReadToList<T>() => MapToList<T>(_reader);

            public T? ReadToValue<T>() where T : struct => MapToValue<T>(_reader);

            public Task<bool> NextResultAsync() => _reader.NextResultAsync();

            public Task<bool> NextResultAsync(CancellationToken ct) => _reader.NextResultAsync(ct);

            public bool NextResult() => _reader.NextResult();

            /// <summary>
            /// Retrieves the column values from the stored procedure and maps them to <typeparamref name="T"/>'s properties
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="dr"></param>
            /// <returns>IList<typeparamref name="T"/></returns>
            private static IList<T> MapToList<T>(DbDataReader dr)
            {
                var objList = new List<T>();
                var props = typeof(T).GetRuntimeProperties().ToArray();

                var colMapping = dr.GetColumnSchema()
                    .Where(x => props.Any(y => string.Equals(y.Name, x.ColumnName, StringComparison.CurrentCultureIgnoreCase)))
                    .ToDictionary(key => key.ColumnName.ToLower());

                if (!dr.HasRows) return objList;

                while (dr.Read())
                {
                    var obj = Activator.CreateInstance<T>();
                    foreach (var prop in props)
                    {
                        colMapping.TryGetValue(prop.Name.ToLower(), out var dbColumn);

                        var property = dbColumn?.ColumnOrdinal;
                        if (property == null) continue;

                        var val = dr.GetValue(property.Value);
                        prop.SetValue(obj, val == DBNull.Value ? null : val);
                    }
                    objList.Add(obj);
                }
                return objList;
            }

            /// <summary>
            ///Attempts to read the first value of the first row of the result set.
            /// </summary>
            private static T? MapToValue<T>(DbDataReader dr) where T : struct
            {
                if (!dr.HasRows) return new T?();

                if (dr.Read())
                {
                    return dr.IsDBNull(0) 
                        ? new T?() 
                        : dr.GetFieldValue<T>(0);
                }
                return new T?();
            }
        }

        /// <summary>
        /// Executes a DbDataReader and returns a list of mapped column values to the properties
        /// </summary>
        /// <param name="command"></param>
        /// <param name="handleResults"></param>
        /// <param name="commandBehaviour"></param>
        /// <returns></returns>
        public static void ExecuteStoredProcedure(this DbCommand command,
            Action<StoredProcedureResults> handleResults,
            CommandBehavior commandBehaviour = CommandBehavior.Default)
        {
            if (handleResults == null)
                throw new ArgumentNullException(nameof(handleResults));

            using (command)
            {
                if (command.Connection.State == ConnectionState.Closed)
                    command.Connection.Open();

                try
                {
                    using (var reader = command.ExecuteReader(commandBehaviour))
                    {
                        var sprocResults = new StoredProcedureResults(reader);

                        handleResults(sprocResults);
                    }
                }
                finally
                {
                    command.Connection.Close();
                }
            }
        }

        /// <summary>
        /// Executes a DbDataReader asynchronously and returns a list of mapped column values to the properties of
        /// </summary>
        /// <param name="command"></param>
        /// <param name="handleResults"></param>
        /// <param name="commandBehaviour"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static async Task ExecuteStoredProcedureAsync(this DbCommand command,
            Action<StoredProcedureResults> handleResults,
            CommandBehavior commandBehaviour = CommandBehavior.Default,
            CancellationToken ct = default(CancellationToken))
        {
            if (handleResults == null)
                throw new ArgumentNullException(nameof(handleResults));

            using (command)
            {
                if (command.Connection.State == ConnectionState.Closed)
                    await command.Connection.OpenAsync(ct).ConfigureAwait(false);

                try
                {
                    using (var reader = await command.ExecuteReaderAsync(commandBehaviour, ct).ConfigureAwait(false))
                    {
                        var spResults = new StoredProcedureResults(reader);
                        handleResults(spResults);
                    }
                }
                finally
                {
                    command.Connection.Close();
                }
            }
        }
    }
}