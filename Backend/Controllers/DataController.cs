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
                var generaloverview = new Dictionary<string, List<string>>
                {
                    {"Year", new List<string> {
                        $"from(bucket: \"{bucket}\") |> range(start: -2y, stop: -1y) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -2y, stop: -1y) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -2y, stop: -1y) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie.*/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -1y) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -1y) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -1y) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie.*/)"
                    }},
                    {"Month", new List<string> {
                        $"from(bucket: \"{bucket}\") |> range(start: -2mo, stop: -1mo) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -2mo, stop: -1mo) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -2mo, stop: -1mo) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie.*/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -1mo) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -1mo) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -1mo) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie.*/)"
                    }},
                    {"Day", new List<string> {
                        $"from(bucket: \"{bucket}\") |> range(start: -2d, stop: -1d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -2d, stop: -1d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -2d, stop: -1d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie.*/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -1d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -1d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -1d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie.*/)"
                    }}
                };

                // Production Overview Queries
                var productionoverview = new Dictionary<string, (string, string, string)>
                {
                    {"Year", ($"from(bucket: \"{bucket}\") |> range(start: -1y) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie_WKK.*/)",
                                $"from(bucket: \"{bucket}\") |> range(start: -1y) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie_PV.*/)",
                                $"from(bucket: \"{bucket}\") |> range(start: -1y) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/)")},
                    {"Month", ($"from(bucket: \"{bucket}\") |> range(start: -1mo) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie_WKK.*/)",
                                $"from(bucket: \"{bucket}\") |> range(start: -1mo) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie_PV.*/)",
                                $"from(bucket: \"{bucket}\") |> range(start: -1mo) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/)")},
                    {"Day", ($"from(bucket: \"{bucket}\") |> range(start: -1d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie_WKK.*/)",
                                $"from(bucket: \"{bucket}\") |> range(start: -1d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie_PV.*/)",
                                $"from(bucket: \"{bucket}\") |> range(start: -1d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/)")}
                };

                var results = new Dictionary<string, List<object>>();

                // General Overview
                results["generaloverview"] = new List<object>();
                foreach (var query in generaloverview)
                {
                    var kortrijkWeideTablesAfnameRef = await client.GetQueryApi().QueryAsync(query.Value[0], org);
                    var kortrijkWeideTablesAfname = await client.GetQueryApi().QueryAsync(query.Value[3], org);
                    var kortrijkWeideTablesProductionRef = await client.GetQueryApi().QueryAsync(query.Value[1], org);
                    var kortrijkWeideTablesProduction = await client.GetQueryApi().QueryAsync(query.Value[4], org);
                    var kortrijkWeideTablesInjectieRef = await client.GetQueryApi().QueryAsync(query.Value[2], org);
                    var kortrijkWeideTablesInjectie = await client.GetQueryApi().QueryAsync(query.Value[5], org);

                    double totalAfnameRef = 0;
                    double totalAfname = 0;
                    double totalProductionRef = 0;
                    double totalProduction = 0;
                    double totalInjectieRef = 0;
                    double totalInjectie = 0;
                    double totalEigenverbruik = 0;
                    double totalEigenverbruikRef = 0;
                    double totalConsumption = 0;
                    double totalConsumptionRef = 0;

                    foreach (var table in kortrijkWeideTablesAfnameRef)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"));
                            totalAfnameRef += value;
                        }
                    }

                    foreach (var table in kortrijkWeideTablesAfname)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"));
                            totalAfname += value;
                        }
                    }

                    foreach (var table in kortrijkWeideTablesProductionRef)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"));
                            totalProductionRef += value;
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

                    foreach (var table in kortrijkWeideTablesInjectieRef)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"));
                            totalInjectieRef += value;
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

                    totalEigenverbruikRef = totalProductionRef - totalInjectieRef;
                    totalEigenverbruik = totalProduction - totalInjectie;
                    totalConsumptionRef = Math.Round(totalAfnameRef + totalEigenverbruikRef, 2);
                    totalConsumption = Math.Round(totalConsumption + totalEigenverbruik, 2);
                    totalProductionRef = Math.Round(totalProductionRef, 2);
                    totalProduction = Math.Round(totalProduction, 2);

                    var data = new
                    {
                        Period = query.Key,
                        Consumption = new { Reference = totalConsumptionRef, Current = totalConsumption },
                        Production = new { Reference = totalProductionRef, Current = totalProduction }
                    };

                    results["generaloverview"].Add(data);
                }

                // Production Overview
                results["productionoverview"] = new List<object>();
                foreach (var query in productionoverview)
                {
                    var kortrijkWeideTablesProduction_WKK = await client.GetQueryApi().QueryAsync(query.Value.Item1, org);
                    var kortrijkWeideTablesProduction_PV = await client.GetQueryApi().QueryAsync(query.Value.Item2, org);
                    var kortrijkWeideTablesInjection = await client.GetQueryApi().QueryAsync(query.Value.Item3, org);

                    double totalProduction_WKK = 0;
                    double totalProduction_PV = 0;
                    double totalInjection = 0;
                    foreach (var table in kortrijkWeideTablesProduction_WKK)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"));
                            totalProduction_WKK += value;
                        }
                    }

                    foreach (var table in kortrijkWeideTablesProduction_PV)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"));
                            totalProduction_PV += value;
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

                    totalProduction_WKK = Math.Round(totalProduction_WKK, 2);
                    totalProduction_PV = Math.Round(totalProduction_PV, 2);
                    totalInjection = Math.Round(totalInjection, 2);

                    var data = new { Period = query.Key, Production_WKK = totalProduction_WKK, Production_PV = totalProduction_PV, Injection = totalInjection };

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
                    {"Year", new List<string> {
                        $"from(bucket: \"{bucket}\") |> range(start: -1y) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/ and r[\"Meter_ID\"] =~ /{buildingName}/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -1y) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/ and r[\"Meter_ID\"] =~ /{buildingName}/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -1y) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie.*/ and r[\"Meter_ID\"] =~ /{buildingName}/)"
                    }},
                    {"Month", new List<string> {
                        $"from(bucket: \"{bucket}\") |> range(start: -1mo) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/ and r[\"Meter_ID\"] =~ /{buildingName}/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -1mo) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/ and r[\"Meter_ID\"] =~ /{buildingName}/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -1mo) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie.*/ and r[\"Meter_ID\"] =~ /{buildingName}/)"
                    }},
                    {"Day", new List<string> {
                        $"from(bucket: \"{bucket}\") |> range(start: -1d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/ and r[\"Meter_ID\"] =~ /{buildingName}/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -1d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/ and r[\"Meter_ID\"] =~ /{buildingName}/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -1d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie.*/ and r[\"Meter_ID\"] =~ /{buildingName}/)"
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

                    var data = new { Period = query.Key, Consumption = totalConsumption, Production = totalProductie };

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
