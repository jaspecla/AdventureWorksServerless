// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using AdventureWorksServerless.Data;
using Newtonsoft.Json;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Core;
using Azure.Identity;
using System.IO;
using Microsoft.Azure.ServiceBus;
using static Microsoft.Azure.ServiceBus.ServiceBusConnectionStringBuilder;
using System.Text;
using Newtonsoft.Json.Serialization;

namespace AdventureWorksServerless
{
  public class ParsePriceUpdateFile
  {
    [FunctionName("ParsePriceUpdateFile")]
    public async Task Run([EventGridTrigger] EventGridEvent eventGridEvent, ILogger log)
    {
      log.LogInformation(eventGridEvent.Data.ToString());

      var blobEvent = JsonConvert.DeserializeObject<BlobEventData>(eventGridEvent.Data.ToString());
    
      if (blobEvent == null)
      {
        log.LogError("Could not parse blob event data from EventGrid event.");
        return;
      }

      var cred = new DefaultAzureCredential();

      var blobClient = new BlobClient(new Uri(blobEvent.Url), cred);
      var response = await blobClient.DownloadAsync();

      var blob = response.Value;

      var builder = new ServiceBusConnectionStringBuilder(Environment.GetEnvironmentVariable("SERVICE_BUS_CONNECTION_STRING"))
      {
        EntityPath = $"/{Environment.GetEnvironmentVariable("PRICE_UPDATE_QUEUE_NAME")}"
      };

      var queueClient = new QueueClient(builder);

      var reader = new StreamReader(blob.Content);
      string line;
      while ((line = await reader.ReadLineAsync()) != null)
      {
        string[] words = line.Split(','); // There are better ways to parse CSV
        var productIdStr = words[0];
        var newPriceStr = words[1];

        int productId;
        decimal newPrice;

        if (!int.TryParse(productIdStr, out productId))
        {
          log.LogError($"Could not get Item ID from line {line}.");
          continue;
        }

        if (!decimal.TryParse(newPriceStr, out newPrice))
        {
          log.LogError($"Could not get New Price from line {line}.");
          continue;
        }

        var update = new PriceUpdate { ProductId = productId, NewPrice = newPrice };
        var updateStr = JsonConvert.SerializeObject(update);
        var message = new Message(Encoding.UTF8.GetBytes(updateStr));

        await queueClient.SendAsync(message);

      }

    }
  }
}
