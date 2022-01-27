using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace lib.dotnet.SharpZipLib
{
    class Program
    {
        static bool proceed = true;

        static async Task Main(string[] args)
        {
            using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
                services.AddTransient<ISharpZipLibMethods, SharpZipLibMethods>()
                )
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
    }
}
