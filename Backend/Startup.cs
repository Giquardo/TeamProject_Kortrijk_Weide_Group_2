using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;

public class Startup
{
    public Startup(IWebHostEnvironment env)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        Configuration = builder.Build();

        LoadEnvironmentVariablesFromEnvFile();
    }

    private void LoadEnvironmentVariablesFromEnvFile()
    {
        var envFile = ".env";
        if (!File.Exists(envFile)) return;

        var lines = File.ReadAllLines(envFile);
        foreach (var line in lines)
        {
            var parts = line.Split('=', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2) continue;

            Configuration[parts[0]] = parts[1];
        }
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IConfiguration>(Configuration);
        services.AddControllers();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowLocalhost3000",
                builder =>
                {
                    builder.WithOrigins("http://localhost:3000")
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500; // or another Status accordingly to Exception Type
                    context.Response.ContentType = "application/json";

                    var error = context.Features.Get<IExceptionHandlerFeature>();
                    if (error != null)
                    {
                        var ex = error.Error;

                        await context.Response.WriteAsync(ex.ToString(), Encoding.UTF8);
                    }
                });
            });
        }

        app.UseRouting();

        app.UseCors("AllowLocalhost3000");

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}