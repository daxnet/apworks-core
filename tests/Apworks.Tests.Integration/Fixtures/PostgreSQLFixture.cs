using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Apworks.Tests.Integration.Fixtures
{
    public class PostgreSQLFixture
    {
        public const string ConnectionString = "User ID=test;Password=test;Host=localhost;Port=5432;Database=test;";

        public PostgreSQLFixture()
        {
            if (CheckTableExists())
            {
                DropTable();
            }

            CreateTable();
        }

        public void ClearTable()
        {
            ExecuteCommand("DELETE FROM public.\"Customers\"");
        }

        private static bool CheckTableExists()
        {
            var sql = "SELECT * FROM information_schema.tables WHERE table_name = 'Customers'";
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
            ExecuteCommand("DROP TABLE public.\"Customers\"");
        }

        private static void CreateTable()
        {
            var createScript = @"
CREATE TABLE public.""Customers""
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
  OWNER TO test;
";
            ExecuteCommand(createScript);
        }

        private static void ExecuteCommand(string sql)
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
    }
}
