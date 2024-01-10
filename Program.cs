using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using DF_EvolutionAPI.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using DF_EvolutionAPI.Models.Response;
using DF_EvolutionAPI.Services.Email;
using DF_EvolutionAPI.Services.History;
using DF_EvolutionAPI.Services.Submission;
using DF_EvolutionAPI.Services;
using System.Configuration;
using Microsoft.Extensions.Configuration;

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
            catch(Exception ex)
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
