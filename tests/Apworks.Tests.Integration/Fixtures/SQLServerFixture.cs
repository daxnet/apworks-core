using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Apworks.Tests.Integration.Fixtures
{
    public class SQLServerFixture
    {
        public static readonly object locker = new object();
        public const string ConnectionString = "Server=localhost;Database=SQLServerEventStoreTest;User Id=sa;Password=G1veMeP@ss";
        public const string ConnectionStringWithoutDatabase = "Server=localhost;User Id=sa;Password=G1veMeP@ss";

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
                var sqlCreateDBQuery = "DROP DATABASE SQLServerEventStoreTest";
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
                    var sqlCreateDBQuery = "SELECT COUNT(*) FROM sys.databases where name = 'SQLServerEventStoreTest'";
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
  FROM SQLServerEventStoreTest.INFORMATION_SCHEMA.TABLES
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

            str = "CREATE DATABASE SQLServerEventStoreTest ON PRIMARY " +
                "(NAME = 'SQLServerEventStoreTest_Data', " +
                "FILENAME = '/var/opt/mssql/data/SQLServerEventStoreTest.mdf', " +
                "SIZE = 2MB, MAXSIZE = 10MB, FILEGROWTH = 10%) " +
                "LOG ON (NAME = 'SQLServerEventStoreTest_Log', " +
                "FILENAME = '/var/opt/mssql/data/SQLServerEventStoreTest.ldf', " +
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
