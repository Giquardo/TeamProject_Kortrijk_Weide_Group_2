using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;

namespace TeamProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DataController> _logger;

        public DataController(IConfiguration configuration, ILogger<DataController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string token = _configuration.GetSection("InfluxDB:Token").Value;
            const string bucket = "Kortrijk Weide";
            const string org = "City of Things";

            _logger.LogInformation($"URL: http://howest-energy-monitoring.westeurope.cloudapp.azure.com:8087");
            _logger.LogInformation($"Token: {token}");

            using var client = InfluxDBClientFactory.Create("http://howest-energy-monitoring.westeurope.cloudapp.azure.com:8087", token);

            var queries = new Dictionary<string, (string, string)>
            {
                {"Year", ($"from(bucket: \"{bucket}\") |> range(start: -90d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/)",
                           $"from(bucket: \"{bucket}\") |> range(start: -90d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/)")},
                {"Month", ($"from(bucket: \"{bucket}\") |> range(start: -30d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/)",
                            $"from(bucket: \"{bucket}\") |> range(start: -30d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/)")},
                {"Day", ($"from(bucket: \"{bucket}\") |> range(start: -216h) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/)",
                          $"from(bucket: \"{bucket}\") |> range(start: -216h) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/)")}
            };

            var results = new List<object>();

            try
            {
                foreach (var query in queries)
                {
                    var kortrijkWeideTablesConsumption = await client.GetQueryApi().QueryAsync(query.Value.Item1, org);
                    var kortrijkWeideTablesProduction = await client.GetQueryApi().QueryAsync(query.Value.Item2, org);

                    double totalConsumption = 0;
                    double totalProduction = 0;
                    foreach (var table in kortrijkWeideTablesConsumption)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"));
                            totalConsumption += value;
                        }
                    }

                    foreach (var table in kortrijkWeideTablesProduction)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"));
                            totalProduction += value;
                        }
                    }

                    totalConsumption = Math.Round(totalConsumption, 2);
                    totalProduction = Math.Round(totalProduction, 2);

                    var data = new { Period = query.Key, Consumption = totalConsumption, Production = totalProduction };

                    results.Add(data);
                }

                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
    }
}