using System;
using System.Configuration;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using WeatherLibrary;

namespace WeatherServiceFunction
{
    public static class RecordWeather
    {
        static int KtoCelsius(double CTempIn)
        {
            double KCel = CTempIn - 273.15;

            return Convert.ToInt32(KCel);
        }
        private static readonly HttpClient client = new HttpClient();
        public static async Task<string> GetWeatherInJson(string apiUrl)
        {
            client.DefaultRequestHeaders.Accept.Clear();

            var stringTask = client.GetStringAsync(apiUrl);
            
            var msg = await stringTask;
            return msg;
        }
        [FunctionName("RecordWeather")]
        public static void Run([TimerTrigger("0 * * * * *")]TimerInfo myTimer,
            [CosmosDB(
                databaseName: "weatherforecast",
                collectionName: "weatherforecastcontainer",
                ConnectionStringSetting = "CosmosDBConnection")]out dynamic document,
            ILogger log)
        {
            string apiUrl = Environment.GetEnvironmentVariable("weatherapiurl").ToString();

            string jsonResult = GetWeatherInJson(apiUrl).Result;
            using var jsonDoc = JsonDocument.Parse(jsonResult);
            var root = jsonDoc.RootElement;

            int listCount = root.GetProperty("list").GetArrayLength();
            if (listCount > 0)
            { 
                var firstElement = root.GetProperty("list")[0]; 

                var mainSection = firstElement.GetProperty("main");
                double kelvinTemp = mainSection.GetProperty("temp").GetDouble(); 

                var weather = firstElement.GetProperty("weather");
                string description = weather[0].GetProperty("description").GetString();

                log.LogInformation($"Inserted "+ KtoCelsius(kelvinTemp).ToString() + " ("+ description + ") at: {DateTime.Now}");

                document = new 
                { temperatureC = KtoCelsius(kelvinTemp), summary = description,  date = DateTime.Now.ToString() };
            }
            else
                document = null;

        }
    }
}
