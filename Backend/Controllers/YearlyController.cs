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
    public class YearlyDataController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<YearlyDataController> _logger;

        public YearlyDataController(IConfiguration configuration, ILogger<YearlyDataController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("generaloverview")]
        public async Task<IActionResult> GetGeneralOverview()
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
                        $"from(bucket: \"{bucket}\") |> range(start: -1y) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -1y) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -1y) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie_WKK.*/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -1y) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie_PV.*/)"
                    }},
                };

                var results = new Dictionary<string, List<object>>();

                // General Overview
                results["generaloverview"] = new List<object>();
                foreach (var query in generaloverview)
                {
                    var kortrijkWeideTablesAfname = await client.GetQueryApi().QueryAsync(query.Value[0], org);
                    var kortrijkWeideTablesInjection = await client.GetQueryApi().QueryAsync(query.Value[1], org);
                    var kortrijkWeideTablesProductionWKK = await client.GetQueryApi().QueryAsync(query.Value[2], org);
                    var kortrijkWeideTablesProductionPV = await client.GetQueryApi().QueryAsync(query.Value[3], org);

                    double totalAfname = 0;
                    double totalProduction = 0;
                    double totalProductionWKK = 0;
                    double totalProductionPV = 0;
                    double totalInjection = 0;
                    double totalConsumption = 0;
                    double totalEigenVerbruik = 0;

                    foreach (var table in kortrijkWeideTablesAfname)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"))/4;
                            totalAfname += value;
                        }
                    }

                    foreach (var table in kortrijkWeideTablesInjection)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"))/4;
                            totalInjection += value;
                        }
                    }

                    foreach (var table in kortrijkWeideTablesProductionWKK)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"))/4;
                            totalProductionWKK += value;
                        }
                    }

                    foreach (var table in kortrijkWeideTablesProductionPV)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"))/4;
                            totalProductionPV += value;
                        }
                    }

                   
                    //afname
                    totalAfname = Math.Round(totalAfname, 2);
                    //injectie
                    totalInjection = Math.Round(totalInjection, 2);
                    //productie
                    totalProductionWKK = Math.Round(totalProductionWKK, 2);
                    totalProductionPV = Math.Round(totalProductionPV, 2);
                    totalProduction = Math.Round(totalProductionWKK + totalProductionPV, 2);
                    //consumptie
                    totalConsumption = Math.Round(totalAfname + (totalProduction - totalInjection), 2);
                    //eigen verbruik
                    totalEigenVerbruik = Math.Round(totalProduction - totalInjection, 2);

                    var data = new
                    {
                        Period = query.Key,
                        Consumption = totalConsumption.ToString("N0"),
                        Production = totalProduction.ToString("N0"),
                        Injection = totalInjection.ToString("N0"),
                        EigenVerbruik = totalEigenVerbruik.ToString("N0")
                    };

                    results["generaloverview"].Add(data);
                }
                return Ok(new
                {
                    generaloverview = results["generaloverview"]
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpGet("productionoverview")]
        public async Task<IActionResult> GetProductionOverview()
        {
            try
            {
                string token = _configuration.GetSection("InfluxDB:Token").Value;
                const string bucket = "Kortrijk Weide";
                const string org = "City of Things";

                using var client = InfluxDBClientFactory.Create("http://howest-energy-monitoring.westeurope.cloudapp.azure.com:8087", token);

                // Production Overview Queries
                var productionoverview = new Dictionary<string, (string, string, string)>
                {
                    {"Year", ($"from(bucket: \"{bucket}\") |> range(start: -1y) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie_WKK.*/)",
                                $"from(bucket: \"{bucket}\") |> range(start: -1y) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie_PV.*/)",
                                $"from(bucket: \"{bucket}\") |> range(start: -1y) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/)")},
                };

                var results = new Dictionary<string, List<object>>();

                // Production Overview
                results["productionoverview"] = new List<object>();
                foreach (var query in productionoverview)
                {
                    var kortrijkWeideTablesProduction_WKK = await client.GetQueryApi().QueryAsync(query.Value.Item1, org);
                    var kortrijkWeideTablesProduction_PV = await client.GetQueryApi().QueryAsync(query.Value.Item2, org);
                    var kortrijkWeideTablesInjection = await client.GetQueryApi().QueryAsync(query.Value.Item3, org);

                    double totalProduction_WKK = 0;
                    double totalProduction_PV = 0;
                    double totalProduction = 0;
                    double totalInjection = 0;

                    foreach (var table in kortrijkWeideTablesProduction_WKK)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"))/4;
                            totalProduction_WKK += value;
                        }
                    }

                    foreach (var table in kortrijkWeideTablesProduction_PV)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"))/4;
                            totalProduction_PV += value;
                        }
                    }

                    foreach (var table in kortrijkWeideTablesInjection)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"))/4;
                            totalInjection += value;
                        }
                    }

                    totalProduction_WKK = Math.Round(totalProduction_WKK, 2);
                    totalProduction_PV = Math.Round(totalProduction_PV, 2);
                    totalProduction = Math.Round(totalProduction_WKK + totalProduction_PV, 2);
                    totalInjection = Math.Round(totalInjection, 2);

                    var data = new
                    {
                        Period = query.Key,
                        Production_WKK = totalProduction_WKK.ToString("N0"),
                        Production_PV = totalProduction_PV.ToString("N0"),
                        Production_Total = totalProduction.ToString("N0"),
                        Injection = totalInjection.ToString("N0")
                    };


                    results["productionoverview"].Add(data);
                }

                return Ok(new
                {
                    productionoverview = results["productionoverview"]
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"An error occurred: {ex.Message}" });
            }
        }
    }
}
