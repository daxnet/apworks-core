using Apworks.Events;
using Apworks.KeyGeneration;
using Apworks.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Apworks.EventStore.AdoNet
{
    public sealed class AdoNetEventStoreConfiguration
    {
        private readonly string tableName;
        private readonly Dictionary<string, string> tableFieldRegistrations = new Dictionary<string, string>();
        private readonly IKeyGenerator<Guid, EventDescriptor> keyGenerator;
        private const string DefaultTableName = "EVENTS";

        public AdoNetEventStoreConfiguration(string connectionString)
            : this(connectionString, new NullKeyGenerator<Guid>())
        { }

        public AdoNetEventStoreConfiguration(string connectionString, IKeyGenerator<Guid, EventDescriptor> keyGenerator)
            : this(connectionString, DefaultTableName, keyGenerator, (KeyValuePair<string, string>[])null)
        { }

        public AdoNetEventStoreConfiguration(string connectionString,
            params KeyValuePair<Expression<Func<EventDescriptor, object>>, string>[] tableFieldRegistrations)
            : this(connectionString, DefaultTableName, tableFieldRegistrations)
        { }

        public AdoNetEventStoreConfiguration(string connectionString, IKeyGenerator<Guid, EventDescriptor> keyGenerator,
            params KeyValuePair<Expression<Func<EventDescriptor, object>>, string>[] tableFieldRegistrations)
            : this(connectionString, DefaultTableName, keyGenerator, tableFieldRegistrations)
        { }

        public AdoNetEventStoreConfiguration(string connectionString,
            string tableName,
            params KeyValuePair<Expression<Func<EventDescriptor, object>>, string>[] tableFieldRegistrations)
            : this(connectionString, tableName, new NullKeyGenerator<Guid>(),
                  tableFieldRegistrations?.ToList()
                    .Select(x => new KeyValuePair<string, string>(Utils.GetPropertyNameFromExpression(x.Key), x.Value)).ToArray())
        { }

        public AdoNetEventStoreConfiguration(string connectionString,
            string tableName,
            IKeyGenerator<Guid, EventDescriptor> keyGenerator,
            params KeyValuePair<Expression<Func<EventDescriptor, object>>, string>[] tableFieldRegistrations)
            : this(connectionString, tableName, keyGenerator,
                  tableFieldRegistrations?.ToList()
                    .Select(x => new KeyValuePair<string, string>(Utils.GetPropertyNameFromExpression(x.Key), x.Value)).ToArray())
        { }

        private AdoNetEventStoreConfiguration(string connectionString,
            string tableName,
            IKeyGenerator<Guid, EventDescriptor> keyGenerator,
            params KeyValuePair<string, string>[] tableFieldRegistrations)
        {
            this.ConnectionString = connectionString;
            this.tableName = tableName;
            this.keyGenerator = keyGenerator;
            InitializeTableFieldDelegationsWithDefaultValues();
            if (tableFieldRegistrations != null)
            {
                foreach (var item in tableFieldRegistrations)
                {
                    // Override the default registration.
                    this.tableFieldRegistrations[item.Key] = item.Value;
                }
            }
        }

        public string ConnectionString { get; }

        public string TableName { get => this.tableName; }

        public string GetFieldName(string propertyName) => this.tableFieldRegistrations.ContainsKey(propertyName) ? this.tableFieldRegistrations[propertyName] : null;

        public string GetFieldName(Expression<Func<EventDescriptor, object>> propertyExpression) => GetFieldName(Utils.GetPropertyNameFromExpression(propertyExpression));

        public bool HasKeyGenerator { get => this.keyGenerator.GetType() != typeof(NullKeyGenerator<Guid>); }

        public Guid GenerateKey(EventDescriptor eventDescriptor) => this.keyGenerator.Generate(eventDescriptor);

        private void InitializeTableFieldDelegationsWithDefaultValues()
        {
            this.tableFieldRegistrations.Clear();
            var properties = from prop in typeof(EventDescriptor)
                                .GetTypeInfo()
                                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                             where prop.CanRead &&
                             (prop.PropertyType.IsSimpleType() || prop.PropertyType == typeof(object))
                             select prop;
            foreach (var p in properties)
            {
                this.tableFieldRegistrations.Add(p.Name, p.Name.ToUpper());
            }
        }

        

    }
}
