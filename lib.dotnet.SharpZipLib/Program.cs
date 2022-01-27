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
        public static int input = default(int);

        static async Task Main(string[] args)
        {
            using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
                services.AddTransient<ISharpZipLibMethods, SharpZipLibMethods>()
                )
            .Build();

            var svc = ActivatorUtilities.CreateInstance<SharpZipLibMethods>(host.Services);
            var cts = new CancellationTokenSource();
            await RT<SharpZipLibMethods>(svc, cts.Token);

            Console.ReadLine();

        }
        static async Task RT<T>(T task, CancellationToken token) where T : ISharpZipLibMethods 
        {
            bool proceed = true;
            if (task == null)
                return;
                while (proceed)
                {
                    proceed = await task.RunProgram();
                }
            
        }
    }
}
