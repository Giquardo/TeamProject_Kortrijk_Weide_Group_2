using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using System.Linq;

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

            // Include all buildings
            List<string> buildingNames = new List<string> { "KWE_A", "KWE_P", "VEG_I_TEC", "LAGO", "Hangar_K", "JC_Tranzit", "MC_Track", "Salie_Tricolor" };

            // Convert the list of building names to a regex pattern
            string buildingNamesPattern = string.Join("|", buildingNames.Select(name => $"{name}"));

            // Zuinige Overview Queries
            var zuinigeoverview = new Dictionary<string, List<string>>
            {
                {soort, new List<string> {
                    $"from(bucket: \"{bucket}\") |> range(start: -1y) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /({buildingNamesPattern})_Productie_{soort}$/) |> aggregateWindow(every: 1mo, fn: sum, createEmpty: false)"
                }}
            };

            var results = new Dictionary<string, Dictionary<string, double>>();

            // Zuinige Overview
            results["zuinigeoverview"] = new Dictionary<string, double>();
            foreach (var query in zuinigeoverview)
            {
                var kortrijkWeideTables = await client.GetQueryApi().QueryAsync(query.Value[0], org);

                foreach (var table in kortrijkWeideTables)
                {
                    foreach (var record in table.Records)
                    {
                        var time = DateTime.Parse(record.GetValueByKey("_time").ToString());
                        var month = time.ToString("MMMM");
                        var value = Convert.ToDouble(record.GetValueByKey("_value")) / 4;

                        if (results["zuinigeoverview"].ContainsKey(month))
                        {
                            results["zuinigeoverview"][month] += value;
                        }
                        else
                        {
                            results["zuinigeoverview"].Add(month, value);
                        }
                    }
                }
            }

            var output = new List<object>();
            foreach (var item in results["zuinigeoverview"])
            {
                output.Add(new
                {
                    Type = soort,
                    Month = item.Key,
                    Production = Math.Round(item.Value).ToString("N0")
                });
            }

            return Ok(new
            {
                zuinigeoverview = output
            });
        }
    }
}