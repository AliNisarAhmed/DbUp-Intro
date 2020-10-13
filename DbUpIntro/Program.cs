using DbUp;
using ServiceStack.OrmLite;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DbUpIntro
{
    public class Tenant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TenantConnectionString { get; set; }
    }

    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var catalogConnectionString = args.FirstOrDefault() ?? "Data Source=.;Initial Catalog=DbUp-catalog;Integrated Security=True";

            // This line creates the db if it does not exist
            EnsureDatabase.For.SqlDatabase(catalogConnectionString);

            // Trying to find a path of the scripts folder
            //string path = Directory.GetCurrentDirectory();
            //var assemblyPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            var upgrader =
                DeployChanges.To
                    .SqlDatabase(catalogConnectionString)
                    //.WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    // TODO: We must get the path to the directory holding the script
                    .WithScriptsFromFileSystem(@"C:\Users\AliAhmed\source\repos\DbUpIntro\DbUpIntro\CatalogScripts")
                    .LogToConsole()
                    .Build();

            string error;

            // We can retry connection here for serverless DBs
            var conn = upgrader.TryConnect(out error);
            if (conn)
            {
                var result = upgrader.PerformUpgrade();

                if (!result.Successful)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(result.Error);
                    Console.ResetColor();
                    Console.ReadLine();
                    return -1;
                }

                var catalogDbFactory = new OrmLiteConnectionFactory(catalogConnectionString, SqlServerDialect.Provider);

                using (var catalogDb = catalogDbFactory.Open())
                {
                    var tenants = await catalogDb.SelectAsync<Tenant>();

                    // For each entry in Tenant table, we run the tenant scripts, DbUp will ensure that scripts run only once

                    // Try adding a script 2.0 in TenantScripts folder

                    foreach (var tenant in tenants)
                    {
                        UpgradeTenantDb(tenant);
                    }
                }

                return 0;
            }
            else
            {
                Console.WriteLine($"Error connecting to Catalog Db {error}");
                return -1;
            }
        }

        static int UpgradeTenantDb(Tenant tenant)
        {
            var tenantConnectionString = tenant.TenantConnectionString;

            EnsureDatabase.For.SqlDatabase(tenantConnectionString);

            var tenantUpgrader =
                DeployChanges.To
                    .SqlDatabase(tenantConnectionString)
                    //.WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    // TODO: We must get the path to the directory holding the script
                    .WithScriptsFromFileSystem(@"C:\Users\AliAhmed\source\repos\DbUpIntro\DbUpIntro\TenantScripts")
                    .LogToConsole()
                    .Build();

            string error;
            var connection = tenantUpgrader.TryConnect(out error);
            if (connection)
            {
                var tenantUpgradeResult = tenantUpgrader.PerformUpgrade();

                if (!tenantUpgradeResult.Successful)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(tenantUpgradeResult.Error);
                    Console.ResetColor();
                    Console.ReadLine();
                    return -1;
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Successful");
                Console.ResetColor();
                return 0;
            }
            else
            {
                Console.WriteLine($"Error connecting to Tenant Db {error}");
                return -1;
            }
        }
    }
}
