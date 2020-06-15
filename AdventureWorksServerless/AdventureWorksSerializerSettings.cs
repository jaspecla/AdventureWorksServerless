using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AdventureWorksServerless
{
  public static class AdventureWorksSerializerSettings
  {
    public static JsonSerializerSettings Get()
    {
      var settings = new JsonSerializerSettings
      {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
      };

      return settings;

    }
  }
}
