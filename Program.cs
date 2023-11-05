using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using DF_EvolutionAPI.Utils;

namespace DF_EvolutionAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var dotenv = Path.Combine(Directory.GetCurrentDirectory(), ".env");
                DotEnv.Load(dotenv);

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
