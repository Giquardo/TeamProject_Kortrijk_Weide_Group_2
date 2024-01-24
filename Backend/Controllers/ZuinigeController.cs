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
    public class ZuinigeDataController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ZuinigeDataController> _logger;

        public ZuinigeDataController(IConfiguration configuration, ILogger<ZuinigeDataController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("zuinigeoverview/{soort}")]
        public async Task<IActionResult> GetZuinigeOverview(string soort)
        {
            if (string.IsNullOrEmpty(soort))
            {
                return BadRequest(new { message = "Parameter 'soort' is required." });
            }

            string token = _configuration.GetSection("InfluxDB:Token").Value;
            const string bucket = "Kortrijk Weide";
            const string org = "City of Things";

            using var client = InfluxDBClientFactory.Create("http://howest-energy-monitoring.westeurope.cloudapp.azure.com:8087", token);


            // Zuinige Overview Queries
            var zuinigeoverview = new Dictionary<string, List<string>>
            {
                {"Week", new List<string> {
                    $"from(bucket: \"{bucket}\") |> range(start: -7d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/)",
                    $"from(bucket: \"{bucket}\") |> range(start: -7d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/)",
                    $"from(bucket: \"{bucket}\") |> range(start: -7d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie_WKK.*/)",
                    $"from(bucket: \"{bucket}\") |> range(start: -7d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Productie_PV.*/)"
                }}
            };
            var results = new Dictionary<string, List<object>>();

            // Zuinige Overview
            results["zuinigeoverview"] = new List<object>();
            foreach (var query in zuinigeoverview)
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
                        var value = Convert.ToDouble(record.GetValueByKey("_value"));
                        totalAfname += value;
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

                foreach (var table in kortrijkWeideTablesProductionWKK)
                {
                    foreach (var record in table.Records)
                    {
                        var value = Convert.ToDouble(record.GetValueByKey("_value"));
                        totalProductionWKK += value;
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

                totalAfname = Math.Round(totalProduction, 2);
                totalInjection = Math.Round(totalInjection, 2);
                totalProductionWKK = Math.Round(totalProductionWKK, 2);
                totalProductionPV = Math.Round(totalProductionPV, 2);
                totalProduction = Math.Round(totalProductionWKK + totalProductionPV, 2);
                totalConsumption = Math.Round(totalAfname + (totalProduction - totalInjection), 2);
                totalEigenVerbruik = Math.Round(totalProduction - totalInjection, 2);

                var data = new object();

                if (soort.ToLower() == "pv")
                {
                    data = new
                    {
                        Type = "PV",
                        Period = query.Key,
                        Consumption = totalConsumption.ToString("N2"),
                        Production = totalProductionPV.ToString("N2"),
                        EigenVerbruik = totalEigenVerbruik.ToString("N2")
                    };
                }
                else if (soort.ToLower() == "wkk")
                {
                    data = new
                    {
                        Type = "WKK",
                        Period = query.Key,
                        Consumption = totalConsumption.ToString("N2"),
                        Production = totalProductionWKK.ToString("N2"),
                        EigenVerbruik = totalEigenVerbruik.ToString("N2")
                    };
                }
                else
                {
                    return BadRequest(new { message = $"Invalid soort parameter: {soort}. It should be either 'PV' or 'WKK'." });
                }

                results["zuinigeoverview"].Add(data);
            }

            return Ok(new
            {
                zuinigeoverview = results["zuinigeoverview"]
            });

        }
    }
}