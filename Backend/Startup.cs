using dotenv.net;
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
using System.Collections.Generic;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IWebHostEnvironment env)
    {
        // Load .env file
        DotEnv.Load();

        var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables(); // This will include the .env variables

        Configuration = builder.Build();
    }

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