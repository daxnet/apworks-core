using Apworks.Events;
using Apworks.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.EventStore.AdoNet
{
    public sealed class AdoNetEventStoreConfiguration
    {
        private readonly string tableName;
        private readonly Dictionary<string, string> tableFieldRegistrations = new Dictionary<string, string>();
        private const string DefaultTableName = "EVENTS";

        public AdoNetEventStoreConfiguration(string connectionString)
            : this(connectionString, DefaultTableName, (KeyValuePair<string, string>[])null)
        { }

        public AdoNetEventStoreConfiguration(string connectionString,
            params KeyValuePair<Expression<Func<EventDescriptor, object>>, string>[] tableFieldRegistrations)
            : this(connectionString, DefaultTableName, tableFieldRegistrations)
        { }

        public AdoNetEventStoreConfiguration(string connectionString,
            string tableName,
            params KeyValuePair<Expression<Func<EventDescriptor, object>>, string>[] tableFieldRegistrations)
            : this(connectionString, tableName,
                  tableFieldRegistrations?.ToList()
                    .Select(x => new KeyValuePair<string, string>(GetPropertyNameFromExpression(x.Key), x.Value)).ToArray())
        { }

        private AdoNetEventStoreConfiguration(string connectionString,
            string tableName,
            params KeyValuePair<string, string>[] tableFieldRegistrations)
        {
            this.ConnectionString = connectionString;
            this.tableName = tableName;
            InitializeTableFieldDelegationsWithDefaultValues();
            if (tableFieldRegistrations != null)
            {
                foreach(var item in tableFieldRegistrations)
                {
                    // Override the default registration.
                    this.tableFieldRegistrations[item.Key] = item.Value;
                }
            }
        }

        public string ConnectionString { get; }

        public string TableName { get => this.tableName; }

        public string GetFieldName(string propertyName) => this.tableFieldRegistrations.ContainsKey(propertyName) ? this.tableFieldRegistrations[propertyName] : null;

        public string GetFieldName(Expression<Func<EventDescriptor, object>> propertyExpression) => GetFieldName(GetPropertyNameFromExpression(propertyExpression));

        private void InitializeTableFieldDelegationsWithDefaultValues()
        {
            this.tableFieldRegistrations.Clear();
            var properties = from prop in typeof(EventDescriptor)
                                .GetTypeInfo()
                                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                             where prop.CanRead &&
                             (prop.PropertyType.IsSimpleType() || prop.PropertyType == typeof(object))
                             select prop;
            foreach(var p in properties)
            {
                this.tableFieldRegistrations.Add(p.Name, p.Name.ToUpper());
            }
        }

        private static string GetPropertyNameFromExpression(Expression<Func<EventDescriptor, object>> expr)
        {
            var memberExpression = expr.Body as MemberExpression;
            return memberExpression?.Member?.Name;
        }
        
    }
}
