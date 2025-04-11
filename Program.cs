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
                    .MinimumLevel.Is(logLevel)
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // Suppress info from ASP.NET, EF, etc.
                    .MinimumLevel.Override("System", LogEventLevel.Warning)                    
                    .Enrich.FromLogContext()
                    .WriteTo.File(new CustomJsonFormatter(), "Logs/log.json", rollingInterval: RollingInterval.Day, retainedFileCountLimit:30)
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
