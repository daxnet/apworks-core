using System;
using System.Linq;
using System.Collections.Generic;
using Apworks.Events;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using Apworks.Utilities;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Apworks.EventStore.AdoNet
{
    public abstract class AdoNetEventStore : Events.EventStore
    {
        private readonly AdoNetEventStoreConfiguration config;

        protected AdoNetEventStore(AdoNetEventStoreConfiguration config, IObjectSerializer payloadSerializer)
            : base(payloadSerializer)
        {
            this.config = config;
        }

        private (string, IEnumerable<IDbDataParameter>) PrepareLoadCommand<TKey>(IDbCommand command, string originatorClrType, TKey originatorId, long sequenceMin, long sequenceMax)
        {
            // Prepare the SQL statement
            var originatorClrTypeParameterName = $"{ParameterChar}{nameof(originatorClrType)}";
            var originatorIdParameterName = $"{ParameterChar}{nameof(originatorId)}";
            var baseSql = $@"SELECT {this.GetEscapedFieldNames()} 
FROM {this.GetEscapedTableName()} 
WHERE {this.GetEscapedFieldNames(propertyExpressions: x => x.OriginatorClrType)}={originatorClrTypeParameterName} 
AND {this.GetEscapedFieldNames(propertyExpressions: x => x.OriginatorId)}={originatorIdParameterName}";

            // Prepare the ADO.NET DbCommand parameter list
            var parameters = new List<IDbDataParameter>();
            parameters.Add(CreateParameter(command, originatorClrTypeParameterName, originatorClrType));
            parameters.Add(CreateParameter(command, originatorIdParameterName, originatorId.ToString())); // Converts the originator Id to string so that database can store.

            // Adding the sequence parameters
            var sqlBuilder = new StringBuilder(baseSql);
            if (sequenceMin > MinimalSequence)
            {
                var sequenceMinParameterName = $"{ParameterChar}{nameof(sequenceMin)}";
                sqlBuilder.Append($" AND {this.GetEscapedFieldNames(propertyExpressions: x => x.EventSequence)}>={sequenceMinParameterName}");
                parameters.Add(CreateParameter(command, sequenceMinParameterName, sequenceMin));
            }

            if (sequenceMax < MaximumSequence)
            {
                var sequenceMaxParameterName = $"{ParameterChar}{nameof(sequenceMax)}";
                sqlBuilder.Append($" AND {this.GetEscapedFieldNames(propertyExpressions: x => x.EventSequence)}<={sequenceMaxParameterName}");
                parameters.Add(CreateParameter(command, sequenceMaxParameterName, sequenceMax));
            }

            sqlBuilder.Append($" ORDER BY {this.GetEscapedFieldNames(propertyExpressions: x => x.EventSequence)}");

            return (sqlBuilder.ToString(), parameters);
        }

        protected override IEnumerable<EventDescriptor> LoadDescriptors<TKey>(string originatorClrType, TKey originatorId, long sequenceMin, long sequenceMax)
        {
            var results = new List<EventDescriptor>();
            using (var connection = this.CreateDatabaseConnection(this.config.ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    (string sql, IEnumerable<IDbDataParameter> parameters) = this.PrepareLoadCommand<TKey>(command, originatorClrType, originatorId, sequenceMin, sequenceMax);
                    command.CommandText = sql;
                    command.Parameters.Clear();
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(this.CreateFromReader(reader));
                        }
                        reader.Close();
                    }
                }
            }

            return results;
        }

        protected override async Task<IEnumerable<EventDescriptor>> LoadDescriptorsAsync<TKey>(string originatorClrType, TKey originatorId, long sequenceMin, long sequenceMax, CancellationToken cancellationToken = default(CancellationToken))
        {
            var results = new List<EventDescriptor>();
            using (var connection = this.CreateDatabaseConnection(this.config.ConnectionString))
            {
                await this.OpenAsync(connection, cancellationToken);
                using (var command = connection.CreateCommand())
                {
                    (string sql, IEnumerable<IDbDataParameter> parameters) = this.PrepareLoadCommand<TKey>(command, originatorClrType, originatorId, sequenceMin, sequenceMax);
                    command.CommandText = sql;
                    command.Parameters.Clear();
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }

                    using (var reader = await ExecuteReaderAsync(command))
                    {
                        while (await ReadAsync(reader))
                        {
                            results.Add(this.CreateFromReader(reader));
                        }

                        reader.Close();
                    }
                }
            }

            return results;
        }

        protected override void SaveDescriptors(IEnumerable<EventDescriptor> descriptors)
        {
            var sql = $"INSERT INTO {this.GetEscapedTableName()} ({this.GetEscapedFieldNames(false)}) VALUES ";
            using (var connection = this.CreateDatabaseConnection(this.config.ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var command = connection.CreateCommand())
                        {
                            var hasCommandPrepared = false;
                            var sortedDescriptors = descriptors.OrderBy(desc => desc.EventTimestamp);
                            foreach (var descriptor in sortedDescriptors)
                            {
                                var parameters = this.GenerateInsertParameters(command, descriptor, false);
                                if (!hasCommandPrepared)
                                {
                                    sql = $"{sql} ({string.Join(", ", parameters.Select(x => x.Key))})";
                                    command.CommandText = sql;
                                    command.Transaction = transaction;
                                    hasCommandPrepared = true;
                                }
                                command.Parameters.Clear();
                                parameters.Select(p => p.Value).ToList().ForEach(p => command.Parameters.Add(p));
                                command.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        protected override async Task SaveDescriptorsAsync(IEnumerable<EventDescriptor> descriptors, CancellationToken cancellationToken = default(CancellationToken))
        {
            var sql = $"INSERT INTO {this.GetEscapedTableName()} ({this.GetEscapedFieldNames(false)}) VALUES ";
            using (var connection = this.CreateDatabaseConnection(this.config.ConnectionString))
            {
                await OpenAsync(connection);
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var command = connection.CreateCommand())
                        {
                            var hasCommandPrepared = false;
                            var sortedDescriptors = descriptors.OrderBy(desc => desc.EventTimestamp);
                            foreach (var descriptor in sortedDescriptors)
                            {
                                var parameters = this.GenerateInsertParameters(command, descriptor, false);
                                if (!hasCommandPrepared)
                                {
                                    sql = $"{sql} ({string.Join(", ", parameters.Select(x => x.Key))})";
                                    command.CommandText = sql;
                                    command.Transaction = transaction;
                                    hasCommandPrepared = true;
                                }
                                command.Parameters.Clear();
                                parameters.Select(p => p.Value).ToList().ForEach(p => command.Parameters.Add(p));
                                await ExecuteNonQueryAsync(command);
                            }
                        }
                        await CommitAsync(transaction);
                    }
                    catch
                    {
                        await RollbackAsync(transaction);
                        throw;
                    }
                }
            }
        }

        #region Database Dialect Overrides
        protected abstract IDbConnection CreateDatabaseConnection(string connectionString);

        protected abstract char ParameterChar { get; }

        protected virtual string BeginLiteralEscapeChar { get => "["; }

        protected virtual string EndLiteralEscapeChar { get => "]"; }

        protected virtual Task OpenAsync(IDbConnection connection, CancellationToken cancellationToken = default(CancellationToken))
            => Task.Factory.StartNew(_ => connection.Open(), cancellationToken);

        protected virtual Task<IDataReader> ExecuteReaderAsync(IDbCommand command, CancellationToken cancellationToken = default(CancellationToken))
            => Task.FromResult(command.ExecuteReader());

        protected virtual Task<bool> ReadAsync(IDataReader reader, CancellationToken cancellationToken = default(CancellationToken))
            => Task.FromResult(reader.Read());

        protected virtual Task<int> ExecuteNonQueryAsync(IDbCommand command, CancellationToken cancellationToken = default(CancellationToken))
            => Task.FromResult(command.ExecuteNonQuery());

        protected virtual Task CommitAsync(IDbTransaction transaction, CancellationToken cancellationToken = default(CancellationToken))
            => Task.Factory.StartNew(_ => transaction.Commit(), cancellationToken);

        protected virtual Task RollbackAsync(IDbTransaction transaction, CancellationToken cancellationToken = default(CancellationToken))
            => Task.Factory.StartNew(_ => transaction.Rollback(), cancellationToken);

        #endregion

        protected AdoNetEventStoreConfiguration Configuration { get => config; }

        protected static IDbDataParameter CreateParameter(IDbCommand command, string parameterName, object parameterValue)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = parameterValue;
            return parameter;
        }

        protected EventDescriptor CreateFromReader(IDataReader reader)
        {
            var eventClrType = (string)reader[this.config.GetFieldName(x => x.EventClrType)];
            var eventType = Type.GetType(eventClrType);

            return new EventDescriptor
            {
                Id = (Guid)reader[this.config.GetFieldName(x => x.Id)],
                EventClrType = eventClrType,
                EventId = (Guid)reader[this.config.GetFieldName(x => x.EventId)],
                EventIntent = (string)reader[this.config.GetFieldName(x => x.EventIntent)],
                EventPayload = this.PayloadSerializer.Deserialize((byte[])reader[this.config.GetFieldName(x => x.EventPayload)], eventType),
                EventTimestamp = (DateTime)reader[this.config.GetFieldName(x => x.EventTimestamp)],
                OriginatorClrType = (string)reader[this.config.GetFieldName(x => x.OriginatorClrType)],
                OriginatorId = (string)reader[this.config.GetFieldName(x => x.OriginatorId)],
                EventSequence = (long)reader[this.config.GetFieldName(x => x.EventSequence)]
            };
        }

        protected string GetEscapedFieldNames(bool includeKeyField = true, params Expression<Func<EventDescriptor, object>>[] propertyExpressions)
        {
            if (propertyExpressions?.Length > 0)
            {
                return string.Join(", ", propertyExpressions
                    .Where(p => includeKeyField || this.config.HasKeyGenerator ? true : Utils.GetPropertyNameFromExpression(p).ToUpper() != "ID")
                    .Select(p => $"{BeginLiteralEscapeChar}{this.config.GetFieldName(p)}{EndLiteralEscapeChar}"));
            }

            return string.Join(", ", typeof(EventDescriptor)
                .GetTypeInfo()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && (p.PropertyType.IsSimpleType() || p.PropertyType == typeof(object)) &&
                    (includeKeyField || this.config.HasKeyGenerator ? true : p.Name.ToUpper() != "ID"))
                .Select(p => $"{BeginLiteralEscapeChar}{this.config.GetFieldName(p.Name)}{EndLiteralEscapeChar}"));
        }

        protected string GetEscapedTableName() => $"{BeginLiteralEscapeChar}{this.config.TableName}{EndLiteralEscapeChar}";

        protected IEnumerable<KeyValuePair<string, IDbDataParameter>> GenerateInsertParameters(IDbCommand command, EventDescriptor eventDescriptor, bool includeKeyField = true)
        {
            return typeof(EventDescriptor)
                .GetTypeInfo()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && (p.PropertyType.IsSimpleType() || p.PropertyType == typeof(object)) &&
                    (includeKeyField || this.config.HasKeyGenerator ? true : p.Name.ToUpper() != "ID"))
                .Select(p =>
                {
                    var parameterName = $"{this.ParameterChar}P_{p.Name}";
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = parameterName;
                    parameter.Value = p.Name == "EventPayload" ?
                        this.PayloadSerializer.Serialize(Type.GetType(eventDescriptor.EventClrType), p.GetValue(eventDescriptor)) :
                        p.GetValue(eventDescriptor);
                    return new KeyValuePair<string, IDbDataParameter>(parameterName, parameter);
                });
        }
    }
}
