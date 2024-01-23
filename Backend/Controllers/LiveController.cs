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
    public class LiveDataController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<LiveDataController> _logger;

        public LiveDataController(IConfiguration configuration, ILogger<LiveDataController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

       [HttpGet("Liveoverview")]
        public async Task<IActionResult> GetLiveOverview()
        {
            try
            {
                string token = _configuration.GetSection("InfluxDB:Token").Value;
                const string bucket = "Kortrijk Weide";
                const string org = "City of Things";
                using var client = InfluxDBClientFactory.Create("http://howest-energy-monitoring.westeurope.cloudapp.azure.com:8087", token);

                // Queries
                var queries = new Dictionary<string, string>
                {
                    {"hour", $"from(bucket: \"{bucket}\") |> range(start: -1h) |> filter(fn: (r) => r[\"_measurement\"] == \"Electricity\")"},
                };

                var results = new Dictionary<string, Dictionary<string, List<object>>>();

                foreach (var query in queries)
                {
                    var tables = await client.GetQueryApi().QueryAsync(query.Value, org);
                    foreach (var table in tables)
                    {
                        foreach (var record in table.Records)
                        {
                            var buildingNameObject = record.GetValueByKey("building");
                            var timestampObject = record.GetValueByKey("_time");
                            var timestampInstant = (NodaTime.Instant)timestampObject;
                            var timestamp = timestampInstant.InUtc().ToDateTimeUtc();
                            var valueObject = record.GetValueByKey("_value");
                            var value = Convert.ToDouble(valueObject);
                            var data = new { Type = "Realtime", Period = query.Key, Consumption = value, Time = timestamp };

                            var key = $"{query.Key}_{timestamp}";

                            if (!results.ContainsKey(key))
                            {
                                results[key] = new Dictionary<string, List<object>>();
                            }

                            var buildingName = buildingNameObject.ToString();
                            if (results[key].ContainsKey(buildingName))
                            {
                                var count = results[key][buildingName].Count;
                                buildingName = $"{buildingName}{count + 1}";
                            }

                            if (!results[key].ContainsKey(buildingName))
                            {
                                results[key][buildingName] = new List<object>();
                            }

                            results[key][buildingName].Add(data);
                        }
                    }
                }

                return Ok(new
                {
                    liveoverview = results
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"An error occurred: {ex.Message}" });
            }
        }
    }
}