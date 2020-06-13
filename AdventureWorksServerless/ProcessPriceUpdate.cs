using System;
using System.Text;
using System.Threading.Tasks;
using AdventureWorksServerless.Data;
using AdventureWorksServerless.Models.Entities;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AdventureWorksServerless
{
  public class ProcessPriceUpdate
  {
    private readonly AdventureWorksDataRepository _repository;
    public ProcessPriceUpdate(AdventureWorksDataRepository repository)
    {
      _repository = repository;
    }

    [FunctionName("ProcessPriceUpdate")]
    public async Task Run([ServiceBusTrigger("%PRICE_UPDATE_QUEUE_NAME%", Connection = "SERVICE_BUS_CONNECTION_STRING")] string priceUpdateQueueMessage, ILogger log)
    {
      log.LogInformation($"Received price update message from queue: {priceUpdateQueueMessage}");

      var errorQueueConnectionStringBuilder = new ServiceBusConnectionStringBuilder(Environment.GetEnvironmentVariable("SERVICE_BUS_CONNECTION_STRING"))
      {
        EntityPath = $"/{Environment.GetEnvironmentVariable("PRICE_UPDATE_ERROR_QUEUE_NAME")}"
      };

      var errorQueueClient = new QueueClient(errorQueueConnectionStringBuilder);


      var priceUpdate = JsonConvert.DeserializeObject<PriceUpdate>(priceUpdateQueueMessage);

      if (priceUpdate == null)
      {
        log.LogError($"Could not parse price update message: {priceUpdateQueueMessage}, sending to error queue.");
        await errorQueueClient.SendAsync(new Message(Encoding.UTF8.GetBytes(priceUpdateQueueMessage)));
        return;
      }

      log.LogInformation($"Updating price of product {priceUpdate.ProductId} to ${priceUpdate.NewPrice}");

      //var productToUpdate = _repository.GetProductFromProductId(priceUpdate.ProductId);

      //if (productToUpdate == null)
      //{
      //  log.LogError($"Could not find product with id ${priceUpdate.ProductId} in the database, sending to error queue.");
      //  await errorQueueClient.SendAsync(new Message(Encoding.UTF8.GetBytes(priceUpdateQueueMessage)));
      //  return;
      //}

      try
      {
        await _repository.UpdateProductWithPriceAsync(priceUpdate.ProductId, priceUpdate.NewPrice);
      }
      catch (Exception ex)
      {
        log.LogError($"Could not update product with id ${priceUpdate.ProductId} to price ${priceUpdate.NewPrice} in the database, sending to error queue: {ex.Message}");
        await errorQueueClient.SendAsync(new Message(Encoding.UTF8.GetBytes(priceUpdateQueueMessage)));
        return;
      }

      log.LogInformation($"Set product with id {priceUpdate.ProductId} to price ${priceUpdate.NewPrice}.");
    }
  }
}
