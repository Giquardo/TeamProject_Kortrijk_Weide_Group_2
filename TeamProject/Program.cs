using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.Configuration;

namespace TeamProject
{
    public class Test
    {
        public static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string token = configuration.GetSection("InfluxDB:Token").Value;
            const string bucket = "Kortrijk Weide";
            const string org = "City of Things";

            using var client = InfluxDBClientFactory.Create("http://howest-energy-monitoring.westeurope.cloudapp.azure.com:8087", token);

            var queries = new Dictionary<string, (string, string)>
            {
                {"Year", ($"from(bucket: \"{bucket}\") |> range(start: -90d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/)",
                           $"from(bucket: \"{bucket}\") |> range(start: -90d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/)")},
                {"Month", ($"from(bucket: \"{bucket}\") |> range(start: -30d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/)",
                            $"from(bucket: \"{bucket}\") |> range(start: -30d) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/)")},
                {"Day", ($"from(bucket: \"{bucket}\") |> range(start: -216h) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Afname$/)",
                          $"from(bucket: \"{bucket}\") |> range(start: -216h) |> filter(fn: (r) => r[\"Meter_ID\"] =~ /.*_Injectie$/)")}
            };

            try
            {
                foreach (var query in queries)
                {
                    var kortrijkWeideTablesConsumption = await client.GetQueryApi().QueryAsync(query.Value.Item1, org);
                    var kortrijkWeideTablesProduction = await client.GetQueryApi().QueryAsync(query.Value.Item2, org);

                    double totalConsumption = 0;
                    double totalProduction = 0;
                    foreach (var table in kortrijkWeideTablesConsumption)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValueByKey("_value"));
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

                    var json = JsonConvert.SerializeObject(data);

                    Console.WriteLine($"Sent data for the {query.Key} to frontend: {json}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}