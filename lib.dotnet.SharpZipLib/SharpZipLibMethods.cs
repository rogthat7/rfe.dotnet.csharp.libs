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
                int fileIndex = 0;
                foreach (var file in files)
                {
                    var info = new FileInfo(file);
                    Console.WriteLine($"{fileIndex++} : {Path.GetFileName(file)}:{info.Length} bytes");
                }
            });
            await t;
        }

        public async Task<bool> RunProgram()
        {
            ConsoleKeyInfo cki;
            System.Console.WriteLine("Input Your Options");
            System.Console.WriteLine("0 : List all avaible zips");
            System.Console.WriteLine("1 : Unzip the zip File");
            System.Console.WriteLine("2 : Zip the target File");
            System.Console.WriteLine("Ctrl+C : Exit the Program");
            cki = Console.ReadKey(true);
            
            switch (cki.KeyChar)
            {
                case '0': {  Console.Clear(); await ListAllZipsInTestData(); break;}
                default: {System.Console.WriteLine("Invalid Key Pressed");  Console.Clear(); break;}
            }
            return true; 
        }

        public async Task UnzipZipFile()
        {
            throw new NotImplementedException();
        }
    }
}