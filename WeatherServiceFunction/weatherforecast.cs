using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;  
using Microsoft.Azure.WebJobs.Host;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.Internal;
using WeatherLibrary;

namespace WeatherServiceFunction
{
    public static class weatherforecast
    {
        private static readonly string[] Summaries = new[]
           {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
          
        [FunctionName("weatherforecast")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "weatherforecast",
                collectionName: "weatherforecastcontainer",
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "SELECT top 10 * FROM c order by c.date desc")]
                IEnumerable<WeatherForecast> wfs,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var rng = new Random();
            List<WeatherForecast> wfList = new List<WeatherForecast>();

            foreach (var item in wfs)
            {
                wfList.Add(new WeatherForecast()
                {
                    date = item.date,
                    temperatureC = item.temperatureC,
                    summary = item.summary
                }
                );
            }
            var obj = JsonConvert.SerializeObject(wfList);
            var returnResult = (ActionResult)new OkObjectResult(obj); 

            return returnResult; 
        }
    }
}
