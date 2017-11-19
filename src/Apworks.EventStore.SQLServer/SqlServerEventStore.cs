using Apworks.EventStore.AdoNet;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Apworks.EventStore.SQLServer
{
    public sealed class SqlServerEventStore : AdoNetEventStore
    {
        public SqlServerEventStore(AdoNetEventStoreConfiguration config, IObjectSerializer payloadSerializer) 
            : base(config, payloadSerializer)
        {
        }

        protected override char ParameterChar => '@';

        protected override IDbConnection CreateDatabaseConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}
