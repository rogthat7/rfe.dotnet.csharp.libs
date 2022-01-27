using System;
using System.IO;
using System.Threading.Tasks;

namespace lib.dotnet.SharpZipLib
{
    public class SharpZipLibMethods : ISharpZipLibMethods
    {
        public async Task ListAllZipsInTestData()
        {
            string path = @"../Test Data";
            var files = Directory.GetFiles(path, "*.zip");

            Task t = Task.Run(() =>
            {
                foreach (var file in files)
                {
                    var info = new FileInfo(file);
                    Console.WriteLine($"{Path.GetFileName(file)}:{info.Length} bytes");
                }
            });
            await t;
        }

        public async Task<bool> RunProgram()
        {
            int.TryParse(Console.ReadLine(), out int input);
            await ListAllZipsInTestData();
            return true; 
        }
    }
}