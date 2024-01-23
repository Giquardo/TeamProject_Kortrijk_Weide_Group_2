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
    public class WeeklyDataController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<WeeklyDataController> _logger;

        public WeeklyDataController(IConfiguration configuration, ILogger<WeeklyDataController> logger)
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
                    {"Week", new List<string> {
                        $"from(bucket: \"{bucket}\") |> range(start: -14d, stop: -7d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -14d, stop: -7d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -14d, stop: -7d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie_WKK.*/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -14d, stop: -7d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie_PV.*/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -7d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -7d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -7d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie_WKK.*/)",
                        $"from(bucket: \"{bucket}\") |> range(start: -7d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie_PV.*/)"
                    }}
                };

                var results = new Dictionary<string, List<object>>();

                // General Overview
                results["generaloverview"] = new List<object>();
                foreach (var query in generaloverview)
                {
                    var kortrijkWeideTablesAfnameRef = await client.GetQueryApi().QueryAsync(query.Value[0], org);
                    var kortrijkWeideTablesInjectionRef = await client.GetQueryApi().QueryAsync(query.Value[1], org);
                    var kortrijkWeideTablesProductionWKKRef = await client.GetQueryApi().QueryAsync(query.Value[2], org);
                    var kortrijkWeideTablesProductionPVRef = await client.GetQueryApi().QueryAsync(query.Value[3], org);
                    var kortrijkWeideTablesAfname = await client.GetQueryApi().QueryAsync(query.Value[4], org);
                    var kortrijkWeideTablesInjection = await client.GetQueryApi().QueryAsync(query.Value[5], org);
                    var kortrijkWeideTablesProductionWKK = await client.GetQueryApi().QueryAsync(query.Value[6], org);
                    var kortrijkWeideTablesProductionPV = await client.GetQueryApi().QueryAsync(query.Value[7], org);
            
                    double totalAfnameRef = 0;
                    double totalAfname = 0;
                    double totalProductionRef = 0;
                    double totalProduction = 0;
                    double totalProductionWKKRef = 0;
                    double totalProductionWKK = 0;
                    double totalProductionPVRef = 0;
                    double totalProductionPV = 0;
                    double totalInjectionRef = 0;
                    double totalInjection = 0;
                    double totalConsumptionRef = 0;
                    double totalConsumption = 0;
                    double totalEigenVerbruik = 0;
        
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

                    foreach (var table in kortrijkWeideTablesInjectionRef)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"));
                            totalInjectionRef += value;
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

                    foreach (var table in kortrijkWeideTablesProductionWKKRef)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"));
                            totalProductionWKKRef += value;
                        }
                    }

                    foreach (var table in kortrijkWeideTablesProductionWKK)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"));
                            totalProductionWKK += value;
                        }
                    }

                     foreach (var table in kortrijkWeideTablesProductionPVRef)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"));
                            totalProductionPVRef += value;
                        }
                    }

                    foreach (var table in kortrijkWeideTablesProductionPV)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"));
                            totalProductionPV += value;
                        }
                    }

                    //afname
                    totalAfnameRef= Math.Round(totalAfnameRef, 2);
                    totalAfname = Math.Round(totalAfname, 2);
                    //injectie
                    totalInjectionRef = Math.Round(totalInjectionRef, 2);
                    totalInjection = Math.Round(totalInjection, 2);
                    //productie
                    totalProductionWKKRef = Math.Round(totalProductionWKKRef, 2);
                    totalProductionPVRef = Math.Round(totalProductionPVRef, 2);
                    totalProductionWKK = Math.Round(totalProductionWKK, 2);
                    totalProductionPV = Math.Round(totalProductionPV, 2);
                    totalProductionRef = Math.Round(totalProductionWKKRef + totalProductionPVRef, 2);
                    totalProduction = Math.Round(totalProductionWKK + totalProductionPV, 2);
                    //consumptie
                    totalConsumptionRef = Math.Round(totalAfnameRef + (totalProductionRef - totalInjectionRef), 2);
                    totalConsumption = Math.Round(totalAfname + (totalProduction - totalInjection), 2);
                    //eigen verbruik
                    totalEigenVerbruik = Math.Round(totalProduction - totalInjection, 2);

                    var data = new { Period = query.Key, ReferenceConsumption = totalConsumptionRef, Consumption = totalConsumption, ReferenceProduction = totalProductionRef, Production = totalProduction, ReferenceInjection = totalInjectionRef, Injection = totalInjection, EigenVerbruik = totalEigenVerbruik};

                    results["generaloverview"].Add(data);
                }
                return Ok(new
                {
                    productionoverview = results["generaloverview"]
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
                    {"Week", ($"from(bucket: \"{bucket}\") |> range(start: -7d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie_WKK.*/)",
                                $"from(bucket: \"{bucket}\") |> range(start: -7d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie_PV.*/)",
                                $"from(bucket: \"{bucket}\") |> range(start: -7d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/)")}
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
                    totalProduction = Math.Round(totalProduction_WKK + totalProduction_PV, 2);
                    totalInjection = Math.Round(totalInjection, 2);

                    var data = new { Period = query.Key, Production_WKK = totalProduction_WKK, Production_PV = totalProduction_PV, Production_Total = totalProduction, Injection = totalInjection };

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
