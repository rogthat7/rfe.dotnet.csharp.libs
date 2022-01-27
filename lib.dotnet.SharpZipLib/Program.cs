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
        static void Main(string[] args)
        {
            using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
                services.AddTransient<ISharpZipLibMethods, SharpZipLibMethods>()
                        .AddTransient<SharpZipLibMethodsTest>()
                )
            .Build();

            RunProgram(host.Services);

        }
        static void RunProgram(IServiceProvider services)
        {
            using IServiceScope serviceScope = services.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;
            CancellationTokenSource wtoken = new CancellationTokenSource();;

            var service = provider.GetRequiredService<SharpZipLibMethodsTest>();
                
            var res = RT<SharpZipLibMethodsTest>(service, wtoken.Token);


        }
        static async Task RT<T>(T task, CancellationToken token) where T : SharpZipLibMethodsTest 
        {
            bool proceed = true;
            if (task == null)
                return;
                while (proceed)
                {
                    Console.WriteLine("Enter Input..");
                    var x = Console.ReadKey();
                    proceed = await task.RunTask(input);
                }
            
        }
    }
}
