using System;
using Serilog;
using System.IO;
using Serilog.Events;
using DF_PA_API.Models;
using DF_EvolutionAPI.Utils;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using DF_EvolutionAPI.Models.Response;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DF_EvolutionAPI
{
    public class Program
    {
        public Program(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public static void Main(string[] args)
        {           
            try
            {          

                var dotenv = Path.Combine(Directory.GetCurrentDirectory(), ".env");
                DotEnv.Load(dotenv);
                // Read log level directly from env
                var logLevelString = Environment.GetEnvironmentVariable("LOG_LEVEL");               
                // Parse string to LogEventLevel (no switch-case)
                var logLevel = (Serilog.Events.LogEventLevel)Enum.Parse(typeof(Serilog.Events.LogEventLevel), logLevelString, ignoreCase: true);

                DotEnv.Load(dotenv);
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Is(logLevel)// Set the minimum log level from the environment
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // Ignore info/debug logs from Microsoft-related logs
                    .MinimumLevel.Override("System", LogEventLevel.Warning)// Same for System-related logs 
                    .WriteTo.File(new CustomJsonFormatter(), "Logs/log.json", rollingInterval: RollingInterval.Day, retainedFileCountLimit:Constant.LOG_DELETION_DAYS)// Rotate file daily. Keep logs for last 10 days
                    .CreateLogger();               

                var builder = WebApplication.CreateBuilder(args);
                builder.Services.Configure<EmailSetting>(builder.Configuration.GetSection("Mail"));

                builder.Services.AddControllersWithViews();

                var app = builder.Build();

                CreateHostBuilder(args).Build().Run();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Application failed to start: " + ex.Message);
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
       
    }
}
