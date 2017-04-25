using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Apworks.Events;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using Apworks.Utilities;

namespace Apworks.EventStore.AdoNet
{
    public abstract class AdoNetEventStore : Events.EventStore
    {
        private readonly AdoNetEventStoreConfiguration config;

        protected AdoNetEventStore(AdoNetEventStoreConfiguration config)
        {
            this.config = config;
        }

        protected override IEnumerable<EventDescriptor> LoadDescriptors<TKey>(string originatorClrType, TKey originatorId)
        {
            var originatorClrTypeParameterName = $"{ParameterChar}{nameof(originatorClrType)}";
            var originatorIdParameterName = $"{ParameterChar}{nameof(originatorId)}";
            var sql = $"SELECT {this.GetEscapedFieldNames()} FROM {this.GetEscapedTableName()} WHERE {this.GetEscapedFieldNames(x => x.OriginatorClrType)}={originatorClrTypeParameterName} AND {this.GetEscapedFieldNames(x => x.OriginatorId)}={originatorIdParameterName}";
            var result = new List<EventDescriptor>();
            using (var connection = this.CreateDatabaseConnection(this.config.ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.Clear();
                    command.Parameters.Add(CreateParameter(command, originatorClrTypeParameterName, originatorClrType));
                    command.Parameters.Add(CreateParameter(command, originatorIdParameterName, originatorId));
                    using (var reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            result.Add(this.CreateFromReader(reader));
                        }
                        reader.Close();
                    }
                }
            }

            return result;
        }

        protected override void SaveDescriptors(IEnumerable<EventDescriptor> descriptors)
        {
            throw new NotImplementedException();
        }

        #region Database Dialect Overrides
        protected abstract IDbConnection CreateDatabaseConnection(string connectionString);

        protected abstract char ParameterChar { get; }

        protected virtual char BeginLiteralEscapeChar { get => '['; }

        protected virtual char EndLiteralEscapeChar { get => ']'; }

        #endregion

        private static IDbDataParameter CreateParameter(IDbCommand command, string parameterName, object parameterValue)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = parameterValue;
            return parameter;
        }

        private EventDescriptor CreateFromReader(IDataReader reader)
        {
            return new EventDescriptor
            {
                Id = (Guid)reader[this.config.GetFieldName(x => x.Id)],
                EventClrType = (string)reader[this.config.GetFieldName(x => x.EventClrType)],
                EventId = (Guid)reader[this.config.GetFieldName(x => x.EventId)],
                EventIntent = (string)reader[this.config.GetFieldName(x => x.EventIntent)],
                EventPayload = reader[this.config.GetFieldName(x => x.EventPayload)],
                EventTimestamp = (DateTime)reader[this.config.GetFieldName(x => x.EventTimestamp)],
                OriginatorClrType = (string)reader[this.config.GetFieldName(x => x.OriginatorClrType)],
                OriginatorId = (string)reader[this.config.GetFieldName(x => x.OriginatorId)]
            };
        }

        private string GetEscapedFieldNames(params Expression<Func<EventDescriptor, object>>[] propertyExpressions)
        {
            if (propertyExpressions?.Length > 0)
            {
                return string.Join(", ", propertyExpressions.Select(p => $"{BeginLiteralEscapeChar}{this.config.GetFieldName(p)}{EndLiteralEscapeChar}"));
            }

            return string.Join(", ", typeof(EventDescriptor)
                .GetTypeInfo()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && (p.PropertyType.IsSimpleType() || p.PropertyType == typeof(object)))
                .Select(p => $"{BeginLiteralEscapeChar}{this.config.GetFieldName(p.Name)}{EndLiteralEscapeChar}"));
        }

        private string GetEscapedTableName() => $"{BeginLiteralEscapeChar}{this.config.TableName}{EndLiteralEscapeChar}";
    }
}
