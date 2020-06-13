using AdventureWorksServerless.Data;
using AdventureWorksServerless.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: FunctionsStartup(typeof(AdventureWorksServerless.Startup))]
namespace AdventureWorksServerless
{
  public class Startup : FunctionsStartup
  {
    public override void Configure(IFunctionsHostBuilder builder)
    {
      builder.Services.AddSingleton<AdventureWorksContextFactory>();
      builder.Services.AddSingleton<AdventureWorksDataRepository>();
    }
  }
}
