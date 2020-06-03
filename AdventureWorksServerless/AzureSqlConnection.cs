using Microsoft.Azure.Services.AppAuthentication;
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
			var resource = "https://database.windows.net/";
			var tokenProvider = new AzureServiceTokenProvider();
			var token = await tokenProvider.GetAccessTokenAsync(resource);

			var builder = new SqlConnectionStringBuilder();
			builder["Data Source"] = $"{dbServer}.database.windows.net";
			builder["Initial Catalog"] = dbName;
			builder["Connect Timeout"] = 30;
			builder["Persist Security Info"] = false;
			builder["TrustServerCertificate"] = false;
			builder["Encrypt"] = true;
			builder["MultipleActiveResultSets"] = false;

			var con = new SqlConnection(builder.ToString());
			con.AccessToken = token;
			return con;
		}
  }
}
