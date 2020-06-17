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

      // NOTE: Since we're using Azure SQL in Serverless mode, we need to implement
      // retry logic.  When the database is in an auto-paused state, the first login will
      // return an error, but will unpause the database for future connections.
      var options = new DbContextOptionsBuilder<AdventureWorksContext>()
        .UseSqlServer(connection, sqlOptions =>
        {
          sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 10,
            maxRetryDelay: TimeSpan.FromSeconds(120),
            errorNumbersToAdd: null
            );
        })
        .Options;

      var dbContext = new AdventureWorksContext(options);

      return dbContext;
    }

  }
}
