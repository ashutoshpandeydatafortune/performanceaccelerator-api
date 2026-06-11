using System;
using System.IO;
using DF_PA_API.Models;
using Serilog;
using Serilog.Events;
using DF_EvolutionAPI.Utils;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace DF_EvolutionAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var dotenv = Path.Combine(Directory.GetCurrentDirectory(), ".env");
            DotEnv.Load(dotenv);
            Log.Logger = CreateLogger();

            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application failed to start");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static ILogger CreateLogger()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

            if (!string.IsNullOrWhiteSpace(environmentName))
            {
                configurationBuilder.AddJsonFile($"appsettings.{environmentName}.json", optional: true);
            }

            var configuration = configurationBuilder
                .AddEnvironmentVariables()
                .Build();

            var logLevelString = Environment.GetEnvironmentVariable("LOG_LEVEL")
                ?? configuration["Logging:LogLevel:Default"]
                ?? "Information";
            var logDeletionDaysString = Environment.GetEnvironmentVariable("LOG_DELETION_DAYS");
            var logLevel = Enum.TryParse(logLevelString, true, out LogEventLevel parsedLevel)
                ? parsedLevel
                : LogEventLevel.Information;
            var logDeletionDays = int.TryParse(logDeletionDaysString, out var parsedValue)
                ? parsedValue
                : Constant.LOG_DELETION_DAYS;

            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Is(logLevel)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .WriteTo.Console();

            if (string.Equals(environmentName, "Development", StringComparison.OrdinalIgnoreCase))
            {
                loggerConfig = loggerConfig.WriteTo.File(
                    new CustomJsonFormatter(),
                    "Logs/log.json",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: logDeletionDays);
            }

            return loggerConfig.CreateLogger();
        }
    }
}
