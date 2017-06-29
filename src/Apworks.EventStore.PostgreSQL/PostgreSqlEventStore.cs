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

        protected override string BeginLiteralEscapeChar => "\"";

        protected override string EndLiteralEscapeChar => "\"";

        protected override async Task CommitAsync(IDbTransaction transaction, CancellationToken cancellationToken = default(CancellationToken))
        {
            await (transaction as NpgsqlTransaction)?.CommitAsync();
        }

        protected override async Task RollbackAsync(IDbTransaction transaction, CancellationToken cancellationToken = default(CancellationToken))
        {
            await (transaction as NpgsqlTransaction)?.RollbackAsync();
        }

        protected override async Task OpenAsync(IDbConnection connection, CancellationToken cancellationToken = default(CancellationToken))
        {
            await (connection as NpgsqlConnection)?.OpenAsync();
        }

        protected override async Task<IDataReader> ExecuteReaderAsync(IDbCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await (command as NpgsqlCommand)?.ExecuteReaderAsync();
        }

        protected override async Task<int> ExecuteNonQueryAsync(IDbCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await (command as NpgsqlCommand)?.ExecuteNonQueryAsync();
        }

        protected override async Task<bool> ReadAsync(IDataReader reader, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await (reader as NpgsqlDataReader)?.ReadAsync();
        }
    }
}
