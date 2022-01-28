using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace lib.dotnet.SharpZipLib
{
    class Program
    {
        static bool proceed = true;

        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);
            // Configure Logger
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
                services.AddTransient<ISharpZipLibMethods, SharpZipLibMethods>()
                )
            .UseSerilog()
            .Build();


            var svc = ActivatorUtilities.CreateInstance<SharpZipLibMethods>(host.Services);
            await RT<SharpZipLibMethods>(svc);

        }
        static async Task RT<T>(T task) where T : ISharpZipLibMethods
        {
            
            Console.CancelKeyPress += new ConsoleCancelEventHandler(myHandler);
            if (task == null)
                return;
            while (proceed)
            {
                proceed = await task.RunProgram();
            }
            
        }
        protected static void myHandler(object sender, ConsoleCancelEventArgs args)
        {
            System.Console.WriteLine("Program Terminating....");
            Task.Delay(TimeSpan.FromSeconds(2));
            System.Environment.Exit(1);
        }

        static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")??"Production"}.json",optional:true)
            .AddEnvironmentVariables();
        }
    }
}
