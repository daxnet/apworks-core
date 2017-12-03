using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace Apworks.Tests.Integration.Fixtures
{
    public class SQLServerFixture
    {
        private const string EventStoreDatabaseName = "SQLServerEventStoreTest";

        public static readonly object locker = new object();
        public static readonly string ConnectionString = $@"Server=localhost\sqlexpress;Database={EventStoreDatabaseName};Integrated Security=SSPI;";
        public const string ConnectionStringWithoutDatabase = @"Server=localhost\sqlexpress;Integrated Security=SSPI;";

        private static readonly string MDF_FileName = Path.Combine(Path.GetTempPath(), EventStoreDatabaseName + ".mdf");
        private static readonly string LDF_FileName = Path.Combine(Path.GetTempPath(), EventStoreDatabaseName + ".ldf");

        public SQLServerFixture()
        {
            if (!CheckDatabase())
            {
                CreateDatabase();
            }
            
            if (!CheckTableExists("EVENTS"))
            {
                CreateTables();
            }
        }

        public void ClearTables()
        {
            ExecuteCommand("DELETE FROM [dbo].[EVENTS]");
        }

        private static void DropDatabase()
        {
            using (var tmpConn = new SqlConnection(ConnectionStringWithoutDatabase))
            {
                var sqlCreateDBQuery = $"DROP DATABASE {EventStoreDatabaseName}";
                tmpConn.Open();
                tmpConn.ChangeDatabase("master");
                using (SqlCommand sqlCmd = new SqlCommand(sqlCreateDBQuery, tmpConn))
                {
                    sqlCmd.ExecuteNonQuery();
                }
            }
        }

        private static bool CheckDatabase()
        {
            try
            {
                using (var tmpConn = new SqlConnection(ConnectionStringWithoutDatabase))
                {
                    var sqlCreateDBQuery = $"SELECT COUNT(*) FROM sys.databases where name = '{EventStoreDatabaseName}'";
                    tmpConn.Open();
                    tmpConn.ChangeDatabase("master");
                    using (SqlCommand sqlCmd = new SqlCommand(sqlCreateDBQuery, tmpConn))
                    {
                        int exists = Convert.ToInt32(sqlCmd.ExecuteScalar());

                        return exists > 0;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool CheckTableExists(string tableName)
        {
            try
            {
                using (var con = new SqlConnection(ConnectionString))
                {
                    var sqlCheck = $@"SELECT COUNT(*)
  FROM {EventStoreDatabaseName}.INFORMATION_SCHEMA.TABLES
  WHERE TABLE_SCHEMA = 'dbo'
  AND TABLE_NAME = '{tableName}'";
                    con.Open();
                    using (var cmd = new SqlCommand(sqlCheck, con))
                    {
                        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        private static void CreateDatabase()
        {
            String str;
            SqlConnection myConn = new SqlConnection(ConnectionStringWithoutDatabase);

            str = $@"CREATE DATABASE {EventStoreDatabaseName} ON PRIMARY " +
                $"(NAME = '{EventStoreDatabaseName}_Data', " +
                $"FILENAME = '{MDF_FileName}', " +
                "SIZE = 2MB, MAXSIZE = 10MB, FILEGROWTH = 10%) " +
                $"LOG ON (NAME = '{EventStoreDatabaseName}_Log', " +
                $"FILENAME = '{LDF_FileName}', " +
                "SIZE = 1MB, " +
                "MAXSIZE = 5MB, " +
                "FILEGROWTH = 10%)";

            SqlCommand myCommand = new SqlCommand(str, myConn);
            try
            {
                myConn.Open();
                myCommand.ExecuteNonQuery();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (myConn.State == ConnectionState.Open)
                {
                    myConn.Close();
                }
            }
        }

        private static void CreateTables()
        {
            var createSql = @"
CREATE TABLE [dbo].[EVENTS](
	[ID] [uniqueidentifier] NOT NULL,
	[EVENTID] [uniqueidentifier] NOT NULL,
	[EVENTTIMESTAMP] [datetime2](7) NOT NULL,
	[EVENTCLRTYPE] [nvarchar](MAX) NOT NULL,
    [EVENTINTENT] [nvarchar](MAX) NOT NULL,
	[ORIGINATORCLRTYPE] [nvarchar](MAX) NOT NULL,
	[ORIGINATORID] [nvarchar](MAX) NOT NULL,
	[EVENTPAYLOAD] [varbinary](max) NOT NULL,
	[EVENTSEQUENCE] [bigint] NOT NULL,
 CONSTRAINT [PK_EVENTS] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

ALTER TABLE [dbo].[EVENTS] ADD  CONSTRAINT [DF_EVENTS_ID]  DEFAULT (newid()) FOR [ID]
";
            ExecuteCommand(createSql);
        }

        private static void ExecuteCommand(string sql)
        {
            try
            {
                using (var con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    using (var cmd = new SqlCommand(sql, con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
