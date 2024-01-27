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
                var dailyLastMonthQuery = $"from(bucket: \"{bucket}\") |> range(start: -1mo) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/ and r[\"Meter_ID\"] =~ /{buildingName}/) |> aggregateWindow(every: 1d, fn: sum, createEmpty: false)";

                var results = new Dictionary<string, List<object>>();

                // Building-Specific Overview
                results["buildingspecificoverview"] = new List<object>();
                results["dailyLastMonth"] = new List<object>();

                var dailyLastMonthTables = await client.GetQueryApi().QueryAsync(dailyLastMonthQuery, org);
                foreach (var table in dailyLastMonthTables)
                {
                    foreach (var record in table.Records)
                    {
                        var value = Convert.ToDouble(record.GetValueByKey("_value")) / 4;
                        var time = record.GetTime();
                        if (time.HasValue)
                        {
                            var dateTimeOffset = time.Value.ToDateTimeOffset();
                            var data = new
                            {
                                Time = dateTimeOffset,
                                Value = Math.Round(value, 2) // Round the value to two decimal places                           
                            };
                            results["dailyLastMonth"].Add(data);
                        }
                    }
                }
                foreach (var query in buildingSpecificQueries)
                {
                    var kortrijkWeideTablesAfname = await client.GetQueryApi().QueryAsync(query.Value[0], org);
                    var kortrijkWeideTablesInjectie = await client.GetQueryApi().QueryAsync(query.Value[1], org);
                    var kortrijkWeideTablesProductie = await client.GetQueryApi().QueryAsync(query.Value[2], org);

                    double totalAfname = 0;
                    double totalInjectie = 0;
                    double totalProductie = 0;
                    double totalConsumption = 0;

                    foreach (var table in kortrijkWeideTablesAfname)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value")) / 4;
                            totalAfname += value;
                        }
                    }
                    foreach (var table in kortrijkWeideTablesInjectie)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value")) / 4;
                            totalInjectie += value;
                        }
                    }
                    foreach (var table in kortrijkWeideTablesProductie)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value")) / 4;
                            totalProductie += value;
                        }
                    }

                    //Productie
                    totalProductie = Math.Round(totalProductie, 2);
                    //Consumption
                    totalConsumption = Math.Round(totalAfname + (totalProductie - totalInjectie), 2);

                    // For the current period
                    var data = new
                    {
                        Type = "Data",
                        Period = query.Key,
                        Consumption = totalConsumption.ToString("N0"),
                        Production = totalProductie.ToString("N0")
                    };
                    results["buildingspecificoverview"].Add(data);
                }

                return Ok(new
                {
                    buildingspecificoverview = results["buildingspecificoverview"],
                    dailyLastMonth = results["dailyLastMonth"]

                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"An error occurred: {ex.Message}" });
            }
        }
    }
}