using Azure.Core;
using Azure.Identity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorksServerless
{
  public static class AzureSqlConnection
  {
    public static async Task<SqlConnection> GetSqlConnectionAsync(string dbServer, string dbName)
		{
			var resource = "https://database.windows.net/.default";
			var cred = new DefaultAzureCredential();
			var token = await cred.GetTokenAsync(new TokenRequestContext(new[] { resource }));

			var builder = new SqlConnectionStringBuilder();
			builder["Data Source"] = $"{dbServer}.database.windows.net";
			builder["Initial Catalog"] = dbName;
			builder["Connect Timeout"] = 30;
			builder["Persist Security Info"] = false;
			builder["TrustServerCertificate"] = false;
			builder["Encrypt"] = true;
			builder["MultipleActiveResultSets"] = false;

			var con = new SqlConnection(builder.ToString());
			con.AccessToken = token.Token;
			return con;
		}
  }
}
