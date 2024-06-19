using DF_EvolutionAPI.Models.Response;
using DF_EvolutionAPI.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

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

                var builder = WebApplication.CreateBuilder(args);
                builder.Services.Configure<EmailSetting>(builder.Configuration.GetSection("Mail"));

                builder.Services.AddControllersWithViews();

                var app = builder.Build();

                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Application failed to start: " + ex.Message);
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

    }
}
