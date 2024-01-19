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
                var buildingSpecificQueries = new Dictionary<string, List<string>>
                {
                    {"Day", new List<string> {
                        $"from(bucket: \"{bucket}\") |> range(start: -1d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/ and r[\"Meter_ID\"] =~ /{buildingName}/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -1d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/ and r[\"Meter_ID\"] =~ /{buildingName}/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -1d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie.*/ and r[\"Meter_ID\"] =~ /{buildingName}/)"
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
                    {"Day", new List<string> {
                        $"from(bucket: \"{bucket}\") |> range(start: -2d, stop: -1d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/ and r[\"Meter_ID\"] =~ /{buildingName}/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -2d, stop: -1d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/ and r[\"Meter_ID\"] =~ /{buildingName}/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -2d, stop: -1d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie.*/ and r[\"Meter_ID\"] =~ /{buildingName}/)"
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
                    var data = new { Type = "Realtime", Period = query.Key, Consumption = totalConsumption, Production = totalProductie };
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
                    var data = new { Type = "Referentie", Period = query.Key, Consumption = previousTotalConsumption, Production = previousTotalProductie };
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

        [HttpGet("overview/weekly")]
        public async Task<IActionResult> GetWeeklyOverview()
        {
            try
            {
                string token = _configuration.GetSection("InfluxDB:Token").Value;
                const string bucket = "Kortrijk Weide";
                const string org = "City of Things";

                using var client = InfluxDBClientFactory.Create("http://howest-energy-monitoring.westeurope.cloudapp.azure.com:8087", token);

                // Weekly Overview Queries
                var weeklyoverview = new Dictionary<string, (string, string, string, string, string, string)>
                {
                    {"Week", ($"from(bucket: \"{bucket}\") |> range(start: -7d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/)",
                                $"from(bucket: \"{bucket}\") |> range(start: -7d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/)",
                                $"from(bucket: \"{bucket}\") |> range(start: -7d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie_EigenVerbruik_WKK$/)",
                                $"from(bucket: \"{bucket}\") |> range(start: -7d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie_EigenVerbruik_PV$/)",
                                $"from(bucket: \"{bucket}\") |> range(start: -7d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie_WKK$/)",
                                $"from(bucket: \"{bucket}\") |> range(start: -7d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie_PV$/)")
                    }
                };

                var results = new List<object>();

                // Weekly Overview
                foreach (var query in weeklyoverview)
                {
                    // Fetch and calculate the total for each variable
                    var kortrijkWeideTablesProductieEigenVerbruikWKK = await client.GetQueryApi().QueryAsync(query.Value.Item3, org);
                    var kortrijkWeideTablesProductieEigenVerbruikPV = await client.GetQueryApi().QueryAsync(query.Value.Item4, org);
                    var kortrijkWeideTablesProductieWKK = await client.GetQueryApi().QueryAsync(query.Value.Item5, org);
                    var kortrijkWeideTablesProductiePV = await client.GetQueryApi().QueryAsync(query.Value.Item6, org);
                    var kortrijkWeideTablesConsumption = await client.GetQueryApi().QueryAsync(query.Value.Item1, org);
                    var kortrijkWeideTablesInjection = await client.GetQueryApi().QueryAsync(query.Value.Item2, org);

                    double totalConsumption = 0;
                    double totalInjection = 0;
                    double totalProductieEigenVerbruikWKK = 0;
                    double totalProductieEigenVerbruikPV = 0;
                    double totalProductieWKK = 0;
                    double totalProductiePV = 0;

                    // Calculate the total for each variable
                    foreach (var table in kortrijkWeideTablesProductieEigenVerbruikWKK)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"));
                            totalProductieEigenVerbruikWKK += value;
                        }
                    }

                    foreach (var table in kortrijkWeideTablesProductieEigenVerbruikPV)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"));
                            totalProductieEigenVerbruikPV += value;
                        }
                    }

                    foreach (var table in kortrijkWeideTablesProductieWKK)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"));
                            totalProductieWKK += value;
                        }
                    }

                    foreach (var table in kortrijkWeideTablesProductiePV)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"));
                            totalProductiePV += value;
                        }
                    }

                    foreach (var table in kortrijkWeideTablesConsumption)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"));
                            totalConsumption += value;
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

                    totalConsumption = Math.Round(totalConsumption, 2);
                    totalInjection = Math.Round(totalInjection, 2);
                    totalProductieEigenVerbruikWKK = Math.Round(totalProductieEigenVerbruikWKK, 2);
                    totalProductieEigenVerbruikPV = Math.Round(totalProductieEigenVerbruikPV, 2);
                    totalProductieWKK = Math.Round(totalProductieWKK, 2);
                    totalProductiePV = Math.Round(totalProductiePV, 2);

                    // Add the totals to the data object
                    var data = new
                    {
                        Period = query.Key,
                        Consumption = totalConsumption.ToString("#,##0.00").Replace(",", "x").Replace(".", ",").Replace("x", "."),
                        Injection = totalInjection.ToString("#,##0.00").Replace(",", "x").Replace(".", ",").Replace("x", "."),
                        TotalProductieEigenVerbruik = (totalProductieEigenVerbruikWKK + totalProductieEigenVerbruikPV).ToString("#,##0.00").Replace(",", "x").Replace(".", ",").Replace("x", "."),
                        TotalProductie = (totalProductieWKK + totalProductiePV).ToString("#,##0.00").Replace(",", "x").Replace(".", ",").Replace("x", "."),
                        TotalProductionProfit = ((totalProductieWKK + totalProductiePV) - (totalProductieEigenVerbruikWKK + totalProductieEigenVerbruikPV)).ToString("#,##0.00").Replace(",", "x").Replace(".", ",").Replace("x", ".")
                    };

                    results.Add(data);
                }

                return Ok(new
                {
                    weeklyoverview = results
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpGet("hernieuwbareEnergie/{soort}")]
        public async Task<IActionResult> GetEnergieSoort(string soort)
        {
            try
            {
                string token = _configuration.GetSection("InfluxDB:Token").Value;
                const string bucket = "Kortrijk Weide";
                const string org = "City of Things";

                using var client = InfluxDBClientFactory.Create("http://howest-energy-monitoring.westeurope.cloudapp.azure.com:8087", token);

                var types = new string[] { "Injectie", "Productie", "Productie_EigenVerbruik" };
                var totals = new Dictionary<string, string>();

                foreach (var type in types)
                {
                    var query = $"from(bucket: \"{bucket}\") |> range(start: -7d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_{type}_{soort}$/)";
                    var tables = await client.GetQueryApi().QueryAsync(query, org);

                    double total = 0;

                    foreach (var table in tables)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"));
                            total += value;
                        }
                    }

                    total = Math.Round(total, 2);
                    totals[type] = total.ToString("#,##0.00").Replace(",", "x").Replace(".", ",").Replace("x", ".");
                }

                return Ok(new { Soort = soort, Totals = totals });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"An error occurred: {ex.Message}" });
            }
        }
    }
}
