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

namespace WeatherServiceFunction
{
    public class WeatherForecast
    { 
        public DateTime date { get; set; }
        public int temperatureC { get; set; }
        public int temperatureF => 32 + (int)(temperatureC / 0.5556);
        public string summary { get; set; }
    }
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
                SqlQuery = "SELECT * FROM c")]
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
             

            //var returnResult = (ActionResult)new OkObjectResult(JsonConvert.SerializeObject(
            //    Enumerable.Range(1, 2).Select(index => new WeatherForecast
            //    {
            //        date = DateTime.Now.AddDays(index),
            //        temperatureC = rng.Next(-20, 55),
            //        summary = Summaries[rng.Next(Summaries.Length)]
            //    })
            //    .ToArray()
            //));


            return returnResult; 
        }
    }
}
