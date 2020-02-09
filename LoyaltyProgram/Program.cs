using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.SystemConsole.Themes;

namespace LoyaltyProgram
{
    //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.1
    public class Program
    {
        //public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
        //    .SetBasePath(Directory.GetCurrentDirectory())
        //    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        //    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        //    .AddEnvironmentVariables()
        //    .Build();
        public static void Main(string[] args)
        {
            //Log.Logger = new LoggerConfiguration()
            //      .ReadFrom.Configuration(Configuration)
            //      .Enrich.FromLogContext()
            //      .WriteTo.Debug()
            //      .WriteTo.Console(theme: AnsiConsoleTheme.Code).CreateLogger();
            // theme: AnsiConsoleTheme.Code
            // new RenderedCompactJsonFormatter()
            // outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")

            try
            {
             //   Log.Information("Getting the motors running...");

                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
              //  Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }

        //  The Generic Host was introduced in 2.1, and was a nice idea, but I found various issues with it, 
        //  primarily as it created more work for libraries.Thankfully this change in 3.0 should solve those issues.
        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.1
        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/web-host?view=aspnetcore-3.1


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
                {
                    // Kestrel is a cross-platform web server. Kestrel is often run in a reverse proxy configuration using IIS.
                    // Kestrel can be run as a public-facing edge server exposed directly to the Internet.
                    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel?view=aspnetcore-3.1
                   // webBuilder.UseKestrel();
                    webBuilder.ConfigureKestrel(serverOptions =>
                    {
                        serverOptions.AllowSynchronousIO = true;
                    });

                    webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                    //webBuilder.UseIIS();
                    //webBuilder.UseUrls("http://myfancydomain:5432", "http://localhost:5101", "http://*:5102");
                    //webBuilder.UseIISIntegration();
                    webBuilder.UseStartup<Startup>();
                });
            //.UseSerilog();
    }
}
