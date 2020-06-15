using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Internal;
using AdventureWorksServerless.Models.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AdventureWorksServerless.Data;

namespace AdventureWorksServerless
{
  public class OrderDetail
  {

    private readonly AdventureWorksDataRepository _repository;

    public OrderDetail(AdventureWorksDataRepository repository)
    {
      _repository = repository;
    }
    [FunctionName("OrderDetail")]
    public async Task<JsonResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
        ILogger log)
    {
      log.LogInformation("OrderDetail function called.");

      string orderNumber = req.Query["orderNumber"];

      if (string.IsNullOrEmpty(orderNumber))
      {
        var result = new JsonResult("OrderDetail must be called with an order number", AdventureWorksSerializerSettings.Get());
        result.StatusCode = StatusCodes.Status400BadRequest;
        return result;
      }

      log.LogInformation($"OrderDetail looking for order {orderNumber}");

      var order =  await _repository.GetOrderFromOrderNumberAsync(orderNumber);

      if (order == null)
      {
        var result = new JsonResult($"Could not find order with order number {orderNumber}", AdventureWorksSerializerSettings.Get());
        result.StatusCode = StatusCodes.Status404NotFound;
        return result;
      }

      return new JsonResult(order, AdventureWorksSerializerSettings.Get());
    }
  }
}
