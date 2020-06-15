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

namespace AdventureWorksServerless
{
  public class GetWeeklySpecials
  {
    private readonly AdventureWorksDataRepository _repository;

    public GetWeeklySpecials(AdventureWorksDataRepository repository)
    {
      _repository = repository;
    }

    [FunctionName("GetWeeklySpecials")]
    public async Task<JsonResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
        ILogger log)
    {
      log.LogInformation("Getting weekly specials.");

      var weeklySpecials = await _repository.GetWeeklySpecials();

      var settings = AdventureWorksSerializerSettings.Get();

      var result = new JsonResult(weeklySpecials, settings);
      return result;
    }
  }
}
