using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AdventureWorksServerless
{
    public static class ParsePriceUpdates
    {
        [FunctionName("ParsePriceUpdates")]
        public static void Run([BlobTrigger("priceupdates/{name}")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }
    }
}
