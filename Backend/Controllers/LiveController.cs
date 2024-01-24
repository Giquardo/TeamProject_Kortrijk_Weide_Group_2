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

        [HttpGet("Liveoverview/{buildingName}")]
        public async Task<IActionResult> GetLiveOverview(string buildingName)
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
                    {"hour", $"from(bucket: \"{bucket}\") |> range(start: -1h) |> filter(fn: (r) => r[\"_measurement\"] == \"Electricity\" and (r[\"msr_subject\"] == \"Building\" or r[\"msr_subject\"] == \"PV\" or r[\"msr_subject\"] == \"WKK\") and r[\"building\"] == \"{buildingName}\") |> sort(columns: [\"_time\"], desc: true) |> limit(n: 1)"},
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
                            var timestampUtc = timestampInstant.InUtc().ToDateTimeUtc();

                            // Convert to Brussels time
                            TimeZoneInfo brusselsTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
                            var timestamp = TimeZoneInfo.ConvertTimeFromUtc(timestampUtc, brusselsTimeZone);

                            var valueObject = record.GetValueByKey("_value");
                            var value = Math.Round(Convert.ToDouble(valueObject), 2);

                            // Get the msr_extra value
                            var msrExtraObject = record.GetValueByKey("msr_extra");
                            var msrExtra = msrExtraObject?.ToString();

                            var data = new
                            {
                                Type = "Realtime",
                                Period = query.Key,
                                Value = value.ToString("N2"),
                                Time = timestamp,
                                MsrExtra = msrExtra
                            };

                            var key = $"{query.Key}_{timestamp}";

                            if (!results.ContainsKey(key))
                            {
                                results[key] = new Dictionary<string, List<object>>();
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