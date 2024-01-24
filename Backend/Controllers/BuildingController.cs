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
    public class BuildingDataController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<BuildingDataController> _logger;

        public BuildingDataController(IConfiguration configuration, ILogger<BuildingDataController> logger)
        {
            _configuration = configuration;
            _logger = logger;
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
                var buildingSpecificQueries = new Dictionary<string, List<string>>
                {
                    {"Week", new List<string> {
                        $"from(bucket: \"{bucket}\") |> range(start: -7d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/ and r[\"Meter_ID\"] =~ /{buildingName}/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -7d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/ and r[\"Meter_ID\"] =~ /{buildingName}/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -7d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie.*/ and r[\"Meter_ID\"] =~ /{buildingName}/)"
                    }},
                    {"Month", new List<string> {
                        $"from(bucket: \"{bucket}\") |> range(start: -1mo) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/ and r[\"Meter_ID\"] =~ /{buildingName}/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -1mo) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/ and r[\"Meter_ID\"] =~ /{buildingName}/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -1mo) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie.*/ and r[\"Meter_ID\"] =~ /{buildingName}/)"
                    }},
                    {"Year", new List<string> {
                        $"from(bucket: \"{bucket}\") |> range(start: -1y) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/ and r[\"Meter_ID\"] =~ /{buildingName}/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -1y) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/ and r[\"Meter_ID\"] =~ /{buildingName}/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -1y) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie.*/ and r[\"Meter_ID\"] =~ /{buildingName}/)"
                    }}
                };
                // Define a new dictionary to store the queries for the previous periods
                var previousPeriodQueries = new Dictionary<string, List<string>>
                {
                    {"Week", new List<string> {
                        $"from(bucket: \"{bucket}\") |> range(start: -14d, stop: -7d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/ and r[\"Meter_ID\"] =~ /{buildingName}/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -14d, stop: -7d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/ and r[\"Meter_ID\"] =~ /{buildingName}/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -14d, stop: -7d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie.*/ and r[\"Meter_ID\"] =~ /{buildingName}/)"
                    }},
                    {"Month", new List<string> {
                        $"from(bucket: \"{bucket}\") |> range(start: -2mo, stop: -1mo) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/ and r[\"Meter_ID\"] =~ /{buildingName}/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -2mo, stop: -1mo) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/ and r[\"Meter_ID\"] =~ /{buildingName}/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -2mo, stop: -1mo) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie.*/ and r[\"Meter_ID\"] =~ /{buildingName}/)"
                    }},
                    {"Year", new List<string> {
                        $"from(bucket: \"{bucket}\") |> range(start: -2y, stop: -1y) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/ and r[\"Meter_ID\"] =~ /{buildingName}/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -2y, stop: -1y) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/ and r[\"Meter_ID\"] =~ /{buildingName}/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -2y, stop: -1y) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie.*/ and r[\"Meter_ID\"] =~ /{buildingName}/)"
                    }}
                };

                var results = new Dictionary<string, List<object>>();
                // Building-Specific Overview
                results["buildingspecificoverview"] = new List<object>();
                foreach (var query in buildingSpecificQueries)
                {
                    var kortrijkWeideTablesAfname = await client.GetQueryApi().QueryAsync(query.Value[0], org);
                    var kortrijkWeideTablesInjectie = await client.GetQueryApi().QueryAsync(query.Value[1], org);
                    var kortrijkWeideTablesProductie = await client.GetQueryApi().QueryAsync(query.Value[2], org);

                    double totalAfname = 0;
                    double totalInjectie = 0;
                    double totalProductie = 0;
                    foreach (var table in kortrijkWeideTablesAfname)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"));
                            totalAfname += value;
                        }
                    }
                    foreach (var table in kortrijkWeideTablesInjectie)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"));
                            totalInjectie += value;
                        }
                    }
                    foreach (var table in kortrijkWeideTablesProductie)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"));
                            totalProductie += value;
                        }
                    }

                    double totalConsumption = totalAfname + (totalProductie - totalInjectie);
                    totalConsumption = Math.Round(totalConsumption, 2);
                    totalProductie = Math.Round(totalProductie, 2);
                    // For the current period
                    var data = new
                    {
                        Type = "Data",
                        Period = query.Key,
                        Consumption = totalConsumption.ToString("N2"),
                        Production = totalProductie.ToString("N2")
                    };
                    results["buildingspecificoverview"].Add(data);
                }
                // Execute the queries for the previous periods and calculate the total consumption and production
                foreach (var query in previousPeriodQueries)
                {
                    var previousTablesAfname = await client.GetQueryApi().QueryAsync(query.Value[0], org);
                    var previousTablesInjectie = await client.GetQueryApi().QueryAsync(query.Value[1], org);
                    var previousTablesProductie = await client.GetQueryApi().QueryAsync(query.Value[2], org);

                    double previousTotalAfname = 0;
                    double previousTotalInjectie = 0;
                    double previousTotalProductie = 0;
                    foreach (var table in previousTablesAfname)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"));
                            previousTotalAfname += value;
                        }
                    }
                    foreach (var table in previousTablesInjectie)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"));
                            previousTotalInjectie += value;
                        }
                    }
                    foreach (var table in previousTablesProductie)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"));
                            previousTotalProductie += value;
                        }
                    }

                    double previousTotalConsumption = previousTotalAfname + (previousTotalProductie - previousTotalInjectie);
                    previousTotalConsumption = Math.Round(previousTotalConsumption, 2);
                    previousTotalProductie = Math.Round(previousTotalProductie, 2);


                    // For the previous period
                    var data = new
                    {
                        Type = "Referentie",
                        Period = query.Key,
                        Consumption = previousTotalConsumption.ToString("N2"),
                        Production = previousTotalProductie.ToString("N2")
                    };
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