using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AdventureWorksServerless.Data;
using System.Text.RegularExpressions;
using AdventureWorksServerless.Models.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AdventureWorksServerless
{
  public class OrderFromEmail
  {

    private readonly AdventureWorksDataRepository _repository;

    public OrderFromEmail(AdventureWorksDataRepository repository)
    {
      _repository = repository;
    }
    [FunctionName("OrderFromEmail")]
    public async Task<JsonResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
      log.LogInformation("OrderFromEmail function called.");

      string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

      var email = JsonConvert.DeserializeObject<EmailContents>(requestBody);

      if (email == null)
      {
        var result = new JsonResult("Could not get email contents from post body.", AdventureWorksSerializerSettings.Get());
        result.StatusCode = StatusCodes.Status400BadRequest;
        return result;
      }

      var salesOrderPattern = @"SO\d{5}\b";
      var salesOrderRegex = new Regex(salesOrderPattern, RegexOptions.IgnoreCase);

      string salesOrderNumber = null;

      // Check the subject first
      var subjectMatch = salesOrderRegex.Match(email.Subject);
      if (subjectMatch.Success)
      {
        salesOrderNumber = subjectMatch.Value;
      }
      else
      {
        var bodyMatch = salesOrderRegex.Match(email.Body);
        if (bodyMatch.Success)
        {
          salesOrderNumber = bodyMatch.Value;
        }
      }

      if (string.IsNullOrEmpty(salesOrderNumber))
      {
        var result = new JsonResult("Could not find a sales order number in the email.", AdventureWorksSerializerSettings.Get());
        result.StatusCode = StatusCodes.Status400BadRequest;
        return result;
      }

      var salesOrder = await _repository.GetOrderFromOrderNumberAsync(salesOrderNumber);

      if (salesOrder == null)
      {
        var result = new JsonResult($"Could not find sales order with order number {salesOrderNumber}", AdventureWorksSerializerSettings.Get());
        result.StatusCode = StatusCodes.Status404NotFound;
        return result;
      }

      return new JsonResult(salesOrder, AdventureWorksSerializerSettings.Get());
    }
  }
}
