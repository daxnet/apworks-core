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
            using (var connection = this.CreateDatabaseConnection(this.config.ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    var originatorClrTypeParameter = command.CreateParameter();
                    originatorClrTypeParameter.ParameterName = originatorClrTypeParameterName;
                    originatorClrTypeParameter.Value = originatorClrType;
                    command.Parameters.Add(originatorClrTypeParameter);

                    return null;
                }
            }
        }

        protected override Task<IEnumerable<EventDescriptor>> LoadDescriptorsAsync<TKey>(string originatorClrType, TKey originatorId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return base.LoadDescriptorsAsync(originatorClrType, originatorId, cancellationToken);
        }

        protected override void SaveDescriptors(IEnumerable<EventDescriptor> descriptors)
        {
            throw new NotImplementedException();
        }

        protected override Task SaveDescriptorsAsync(IEnumerable<EventDescriptor> descriptors, CancellationToken cancellationToken = default(CancellationToken))
        {
            return base.SaveDescriptorsAsync(descriptors, cancellationToken);
        }

        #region Database Dialect Overrides
        protected abstract IDbConnection CreateDatabaseConnection(string connectionString);

        protected abstract char ParameterChar { get; }

        protected virtual char BeginLiteralEscapeChar { get => '['; }

        protected virtual char EndLiteralEscapeChar { get => ']'; }

        #endregion

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
