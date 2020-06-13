using AdventureWorksServerless.Models.Entities;
using Azure.Core;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorksServerless.Data
{
  public class AdventureWorksContextFactory
  {

    private readonly string _connectionString;

    public AdventureWorksContextFactory()
    {
      var dbHostName = Environment.GetEnvironmentVariable("AZURE_SQL_HOST_NAME");
      var dbName = Environment.GetEnvironmentVariable("AZURE_SQL_DB_NAME");


      var builder = new SqlConnectionStringBuilder();
      builder.DataSource = $"{dbHostName}.database.windows.net";
      builder.InitialCatalog = dbName;
      builder.ConnectTimeout = 30;
      builder.PersistSecurityInfo = false;
      builder.TrustServerCertificate = false;
      builder.Encrypt = true;
      builder.MultipleActiveResultSets = true;

      _connectionString = builder.ToString();
    }

    private async Task<SqlConnection> CreateSqlConnection()
    {
      var resource = "https://database.windows.net/.default";
      var cred = new DefaultAzureCredential();
      var token = await cred.GetTokenAsync(new TokenRequestContext(new[] { resource }));

      var con = new SqlConnection(_connectionString);
      con.AccessToken = token.Token;
      return con;

    }

    public async Task<AdventureWorksContext> CreateAsync()
    {
      var connection = await CreateSqlConnection();

      var options = new DbContextOptionsBuilder<AdventureWorksContext>()
        .UseSqlServer(connection)
        .Options;

      var dbContext = new AdventureWorksContext(options);

      return dbContext;
    }

  }
}
