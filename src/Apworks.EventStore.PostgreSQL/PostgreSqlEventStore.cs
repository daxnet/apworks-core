using Apworks.EventStore.AdoNet;
using Npgsql;
using System;
using System.Linq;
using System.Data;
using Apworks.Events;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.EventStore.PostgreSQL
{
    public sealed class PostgreSqlEventStore : AdoNetEventStore
    {
        public PostgreSqlEventStore(AdoNetEventStoreConfiguration config, IObjectSerializer payloadSerializer)
            : base(config, payloadSerializer)
        { }

        protected override char ParameterChar => '@';

        protected override IDbConnection CreateDatabaseConnection(string connectionString)
        {
            return new NpgsqlConnection(connectionString);
        }

        protected override async Task<IEnumerable<EventDescriptor>> LoadDescriptorsAsync<TKey>(string originatorClrType, TKey originatorId, long sequenceMin, long sequenceMax, CancellationToken cancellationToken = default(CancellationToken))
        {
            var originatorClrTypeParameterName = $"{ParameterChar}{nameof(originatorClrType)}";
            var originatorIdParameterName = $"{ParameterChar}{nameof(originatorId)}";
            var sql = $"SELECT {this.GetEscapedFieldNames()} FROM {this.GetEscapedTableName()} WHERE {this.GetEscapedFieldNames(propertyExpressions: x => x.OriginatorClrType)}={originatorClrTypeParameterName} AND {this.GetEscapedFieldNames(propertyExpressions: x => x.OriginatorId)}={originatorIdParameterName}";
            var results = new List<EventDescriptor>();
            using (var connection = (NpgsqlConnection)this.CreateDatabaseConnection(this.Configuration.ConnectionString))
            {
                await connection.OpenAsync(cancellationToken);
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.Clear();
                    command.Parameters.Add(CreateParameter(command, originatorClrTypeParameterName, originatorClrType));
                    command.Parameters.Add(CreateParameter(command, originatorIdParameterName, originatorId));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            results.Add(this.CreateFromReader(reader));
                        }
                    }
                }
            }

            return results;
        }

        protected override async Task SaveDescriptorsAsync(IEnumerable<EventDescriptor> descriptors, CancellationToken cancellationToken = default(CancellationToken))
        {
            var sql = $"INSERT INTO {this.GetEscapedTableName()} ({this.GetEscapedFieldNames(false)}) VALUES ";
            using (var connection = (NpgsqlConnection)this.CreateDatabaseConnection(this.Configuration.ConnectionString))
            {
                await connection.OpenAsync();
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
                                await command.ExecuteNonQueryAsync();
                            }
                        }
                        await transaction.CommitAsync();
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        protected override string BeginLiteralEscapeChar => "\"";

        protected override string EndLiteralEscapeChar => "\"";
    }
}
