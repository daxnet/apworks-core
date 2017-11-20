using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;

namespace Apworks.Tests.Integration.Fixtures
{
    public class PostgreSQLFixture
    {
        public static readonly object locker = new object();

        public const string ConnectionString = "User ID=test;Password=oe9jaacZLbR9pN;Host=localhost;Port=5432;Database=test;";

        private const string CreateAddressTableSql = @"CREATE TABLE ""Addresses"" (
    ""Id"" serial NOT NULL,
    ""City"" text,
    ""Country"" text,
    ""CustomerId"" int4,
    ""State"" text,
    ""Street"" text,
    ""ZipCode"" text,
    CONSTRAINT ""PK_Addresses"" PRIMARY KEY(""Id""),
    CONSTRAINT ""FK_Addresses_Customers_CustomerId"" FOREIGN KEY(""CustomerId"") REFERENCES ""Customers""(""Id"") ON DELETE NO ACTION
);

CREATE INDEX ""IX_Addresses_CustomerId"" ON ""Addresses"" (""CustomerId"");

ALTER TABLE public.""Addresses""
  OWNER TO test;";

        private const string CreateCustomerTableSql = @"CREATE TABLE public.""Customers""
(
  ""Id"" integer NOT NULL,
  ""Email"" text,
  ""Name"" text,
  CONSTRAINT ""PK_Customers"" PRIMARY KEY(""Id"")
)
WITH(
  OIDS = FALSE
);

ALTER TABLE public.""Customers""
  OWNER TO test;";

        private const string CreateEventsTableSql = @"CREATE TABLE public.""EVENTS""
(
    ""ID"" uuid NOT NULL,
    ""EVENTID"" uuid NOT NULL,
    ""EVENTTIMESTAMP"" timestamp without time zone NOT NULL,
    ""EVENTCLRTYPE"" text COLLATE pg_catalog.""default"" NOT NULL,
    ""EVENTINTENT"" text COLLATE pg_catalog.""default"" NOT NULL,
    ""ORIGINATORCLRTYPE"" text COLLATE pg_catalog.""default"" NOT NULL,
    ""ORIGINATORID"" text COLLATE pg_catalog.""default"" NOT NULL,
    ""EVENTPAYLOAD"" bytea NOT NULL,
    ""EVENTSEQUENCE"" bigint NOT NULL,
    CONSTRAINT ""EVENTS_pkey"" PRIMARY KEY(""ID"")
);

ALTER TABLE public.""EVENTS""
    OWNER TO test;";

        public PostgreSQLFixture()
        {
            if (!CheckTableExists("Customers"))
                CreateTable("Customers");

            if (!CheckTableExists("Addresses"))
                CreateTable("Addresses");

            if (!CheckTableExists("EVENTS"))
                CreateTable("EVENTS");
        }

        public void ClearTables()
        {
            ExecuteCommand("DELETE FROM public.\"Addresses\"");
            ExecuteCommand("DELETE FROM public.\"Customers\"");
            ExecuteCommand("DELETE FROM public.\"EVENTS\"");
        }

        private static bool CheckTableExists(string tableName)
        {
            var sql = $"SELECT * FROM information_schema.tables WHERE table_name = '{tableName}'";
            var tableExists = false;
            using (var con = new NpgsqlConnection(ConnectionString))
            {
                using (var cmd = new NpgsqlCommand(sql))
                {
                    if (cmd.Connection == null)
                        cmd.Connection = con;
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    lock (cmd)
                    {
                        using (NpgsqlDataReader rdr = cmd.ExecuteReader())
                        {
                            try
                            {
                                if (rdr != null && rdr.HasRows)
                                    tableExists = true;
                                else
                                    tableExists = false;
                            }
                            catch
                            {
                                tableExists = false;
                            }
                        }
                    }
                }
            }
            return tableExists;
        }

        private static void DropTable()
        {
            ExecuteCommand("DROP TABLE public.\"Addresses\"");
            ExecuteCommand("DROP TABLE public.\"Customers\"");
            ExecuteCommand("DROP TABLE public.\"EVENTS\"");
        }

        private static void CreateTable(string tableName)
        {
            var createScript = "";
            switch (tableName.ToLower())
            {
                case "customers":
                    createScript = CreateCustomerTableSql;
                    break;
                case "addresses":
                    createScript = CreateAddressTableSql;
                    break;
                case "events":
                    createScript = CreateEventsTableSql;
                    break;
            }

            ExecuteCommand(createScript);
        }

        private static void ExecuteCommand(string sql)
        {
            try
            {
                using (var con = new NpgsqlConnection(ConnectionString))
                {
                    con.Open();
                    using (var cmd = new NpgsqlCommand(sql, con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (PostgresException)
            {  }
        }
    }
}
