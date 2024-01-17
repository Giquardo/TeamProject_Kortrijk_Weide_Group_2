using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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

        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview()
        {
            try
            {
                string token = _configuration.GetSection("InfluxDB:Token").Value;
                const string bucket = "Kortrijk Weide";
                const string org = "City of Things";

                using var client = InfluxDBClientFactory.Create("http://howest-energy-monitoring.westeurope.cloudapp.azure.com:8087", token);

                // General Overview Queries
                var generaloverview = new Dictionary<string, (string, string)>
                {
                    {"Year", ($"from(bucket: \"{bucket}\") |> range(start: -90d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/)",
                                $"from(bucket: \"{bucket}\") |> range(start: -90d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie.*/)")},
                    {"Month", ($"from(bucket: \"{bucket}\") |> range(start: -30d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/)",
                                    $"from(bucket: \"{bucket}\") |> range(start: -30d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie.*/)")},
                    {"Day", ($"from(bucket: \"{bucket}\") |> range(start: -11d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/)",
                                $"from(bucket: \"{bucket}\") |> range(start: -11d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie.*/)")}
                };
                
                // Production Overview Queries
                var productionoverview = new Dictionary<string, (string, string)>
                {
                    {"Year", ($"from(bucket: \"{bucket}\") |> range(start: -90d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie.*/)",
                                $"from(bucket: \"{bucket}\") |> range(start: -90d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/)")},
                    {"Month", ($"from(bucket: \"{bucket}\") |> range(start: -30d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie.*/)",
                                    $"from(bucket: \"{bucket}\") |> range(start: -30d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/)")},
                    {"Day", ($"from(bucket: \"{bucket}\") |> range(start: -11d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie.*/)",
                                $"from(bucket: \"{bucket}\") |> range(start: -11d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/)")}
                };

                var results = new Dictionary<string, List<object>>();

                // General Overview
                results["generaloverview"] = new List<object>();
                foreach (var query in generaloverview)
                {
                    var kortrijkWeideTablesConsumption = await client.GetQueryApi().QueryAsync(query.Value.Item1, org);
                    var kortrijkWeideTablesProduction = await client.GetQueryApi().QueryAsync(query.Value.Item2, org);

                    double totalConsumption = 0;
                    double totalProduction = 0;
                    foreach (var table in kortrijkWeideTablesConsumption)
                    {
                        foreach (var record in table.Records)
                        {
                            var meterId = record.GetValueByKey("_field")?.ToString(); // Check for null
                            var value = Convert.ToDouble(record.GetValueByKey("_value"));

                            // Apply regex for Consumption (Afname)
                            var consumptionMatch = Regex.Match(meterId ?? "", @"_(.*?)_Afname$");
                            if (consumptionMatch.Success)
                            {
                                var period = query.Key;
                                var buildingName = consumptionMatch.Groups[1].Value;
                                Console.WriteLine($"Period: {period}, Building: {buildingName}, Consumption: {value}");
                            }

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

                    results["generaloverview"].Add(data);
                }

                // Production Overview
                results["productionoverview"] = new List<object>();
                foreach (var query in productionoverview)
                {
                    var kortrijkWeideTablesProduction = await client.GetQueryApi().QueryAsync(query.Value.Item1, org);
                    var kortrijkWeideTablesInjection = await client.GetQueryApi().QueryAsync(query.Value.Item2, org);

                    double totalProduction = 0;
                    double totalInjection = 0;
                    foreach (var table in kortrijkWeideTablesProduction)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"));
                            totalProduction += value;
                        }
                    }

                    foreach (var table in kortrijkWeideTablesInjection)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"));
                            totalInjection += value;
                        }
                    }

                    totalProduction = Math.Round(totalProduction, 2);
                    totalInjection = Math.Round(totalInjection, 2);

                    var data = new { Period = query.Key, Production = totalProduction, Injection = totalInjection };

                    results["productionoverview"].Add(data);
                }

                return Ok(new
                {
                    generaloverview = results["generaloverview"],
                    productionoverview = results["productionoverview"],
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpGet("buildingspecific/{buildingName}")]
        public async Task<IActionResult> GetBuildingSpecificOverview(string buildingName)
        {
            try
            {
                string token = _configuration.GetSection("InfluxDB:Token").Value;
                const string bucket = "Kortrijk Weide";
                const string org = "City of Things";

                using var client = InfluxDBClientFactory.Create("http://howest-energy-monitoring.westeurope.cloudapp.azure.com:8087", token);

                // Building-Specific Queries
               var buildingSpecificQueries = new Dictionary<string, (string, string)>
                {
                    {"Year", ($"from(bucket: \"{bucket}\") |> range(start: -90d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/ and r[\"Meter_ID\"] =~ /{buildingName}/)",
                            $"from(bucket: \"{bucket}\") |> range(start: -90d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie.*/ and r[\"Meter_ID\"] =~ /{buildingName}/)")},
                    {"Month", ($"from(bucket: \"{bucket}\") |> range(start: -30d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/ and r[\"Meter_ID\"] =~ /{buildingName}/)",
                                $"from(bucket: \"{bucket}\") |> range(start: -30d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie.*/ and r[\"Meter_ID\"] =~ /{buildingName}/)")},
                    {"Day", ($"from(bucket: \"{bucket}\") |> range(start: -11d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/ and r[\"Meter_ID\"] =~ /{buildingName}/)",
                            $"from(bucket: \"{bucket}\") |> range(start: -11d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie.*/ and r[\"Meter_ID\"] =~ /{buildingName}/)")}
                };

                var results = new Dictionary<string, List<object>>();

                // Building-Specific Overview
                results["buildingspecificoverview"] = new List<object>();
                foreach (var query in buildingSpecificQueries)
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

                    results["buildingspecificoverview"].Add(data);
                }

                return Ok(new
                {
                    buildingspecificoverview = results["buildingspecificoverview"]
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"An error occurred: {ex.Message}" });
            }
        }
    }
}
