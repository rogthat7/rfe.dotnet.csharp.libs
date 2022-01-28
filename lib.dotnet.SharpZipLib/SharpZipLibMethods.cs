using System;
using System.IO;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Extensions.Logging;

namespace lib.dotnet.SharpZipLib
{
    public class SharpZipLibMethods : ISharpZipLibMethods
    {
        const string TEST_PATH = @"../Test Data";
        private readonly ILogger<SharpZipLibMethods> _logger;
        public SharpZipLibMethods(ILogger<SharpZipLibMethods> logger)
        {
            _logger = logger;
        }

        public async Task<string[]> ListAllZipsInTestData()
        {
            var files = Directory.GetFiles(TEST_PATH, "*.zip");

            Task t = Task.Run(() =>
            {
                ListAllZips(files);
            });
            await t;
            return files;
        }

        private void ListAllZips(string[] files)
        {
            int fileIndex = 0;
            if (files.Length == 0)
                System.Console.WriteLine("No Zips Available");
            foreach (var file in files)
            {
                var info = new FileInfo(file);
                Console.WriteLine($"{fileIndex++} : {Path.GetFileName(file)}:{info.Length} bytes");
            }
        }

        private DirectoryInfo? ListAllDirectories(string[] dirs)
        {
            int dirIndex = 0;
            if (dirs.Length == 0)
                System.Console.WriteLine("No Directories Available");
            foreach (var dir in dirs)
            {
                var info = new DirectoryInfo(dir);
                Console.WriteLine($"{dirIndex++} : {Path.GetFileName(dir)}");
            }
            Console.WriteLine($"{dirIndex++} : All Directories");

            System.Console.WriteLine("Select the Directory to Zip!");
            var cki = Console.ReadKey(true);
            int.TryParse(cki.KeyChar.ToString(), out int option);

            return option >= dirs.Length ? new DirectoryInfo(dirs[0]).Parent  : new DirectoryInfo(dirs[option]);
        }

        public async Task<bool> RunProgram()
        {
            ConsoleKeyInfo cki;
            System.Console.WriteLine("Input Your Options");
            System.Console.WriteLine("0 : List all avaible zips");
            System.Console.WriteLine("1 : Unzip the zip File");
            System.Console.WriteLine("2 : Zip the Exisitng Directory");
            System.Console.WriteLine("Ctrl+C : Exit the Program");
            cki = Console.ReadKey(true);
            int.TryParse(cki.KeyChar.ToString(), out int option);
            switch (option)
            {
                case 0: { Console.Clear(); await ListAllZipsInTestData(); break; }
                case 1: { Console.Clear(); await UnzipZipFile(); break; }
                case 2: { Console.Clear(); await ZipDirectoryWithPassword(); break; }
                default: { System.Console.WriteLine("Invalid Invalid"); break; }
            }
            return true;
        }

        /// <summary>
        /// Extracts the content from a .zip file inside an specific folder.
        /// </summary>
        public async Task UnzipZipFile()
        {
            var filesArray = await ListAllZipsInTestData();
            if (filesArray.Length == 0)
                return;
            System.Console.WriteLine("Select the File to Unzip!");
            var cki = Console.ReadKey(true);
            int.TryParse(cki.KeyChar.ToString(), out int option);
            if (option >= filesArray.Length)
            {
                _logger.LogError(new InvalidOperationException().Message);
                return;
            }
            System.Console.WriteLine("Enter The Password/Press Enter if No Password");
            var password = Console.ReadLine();

            ZipFile file = null;
            try
            {
                FileStream fs = File.OpenRead(Path.Join(TEST_PATH, filesArray.ElementAt(option)));
                file = new ZipFile(fs);

                if (!String.IsNullOrEmpty(password))
                {
                    // AES encrypted entries are handled automatically
                    file.Password = password;
                }

                foreach (ZipEntry zipEntry in file)
                {
                    if (!zipEntry.IsFile)
                    {
                        // Ignore directories
                        continue;
                    }

                    String entryFileName = zipEntry.Name;
                    // to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
                    // Optionally match entrynames against a selection list here to skip as desired.
                    // The unpacked length is available in the zipEntry.Size property.

                    // 4K is optimum
                    byte[] buffer = new byte[4096];
                    Stream zipStream = file.GetInputStream(zipEntry);

                    // Manipulate the output filename here as desired.
                    String fullZipToPath = Path.Combine(TEST_PATH,Path.GetFileNameWithoutExtension(filesArray[option]) ,entryFileName);
                    string directoryName = Path.GetDirectoryName(fullZipToPath);

                    if (directoryName.Length > 0)
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                    // of the file, but does not waste memory.
                    // The "using" will close the stream even if an exception occurs.
                    using (FileStream streamWriter = File.Create(fullZipToPath))
                    {
                        StreamUtils.Copy(zipStream, streamWriter, buffer);
                    }
                }
            }
            finally
            {
                if (file != null)
                {
                    file.IsStreamOwner = true; // Makes close also shut the underlying stream
                    file.Close(); // Ensure we release resources
                }
            }
        }

        /// <summary>
        /// Method that compress all the files inside a folder (non-recursive) into a zip file.
        /// </summary>
        private async Task ZipDirectoryWithPassword()
        {
            try
            {
                var dirs = Directory.GetDirectories(TEST_PATH);
                // Depending on the directory this could be very large and would require more attention
                // in a commercial package.
                var selectedDir = ListAllDirectories(dirs);

                // 'using' statements guarantee the stream is closed properly which is a big source
                // of problems otherwise.  Its exception safe as well which is great.
                using (ZipOutputStream OutputStream = new ZipOutputStream(File.Create(string.Concat(TEST_PATH,'/',selectedDir.Name)+".zip")))
                {
                    // Define a password for the file (if providen)
                    // set its value to null or don't declare it to leave the file
                    // without password protection
                    System.Console.WriteLine("Enter The Password/Press Enter if No Password");
                    var Password = Console.ReadLine();
                    OutputStream.Password = Password;

                    // Define the compression level
                    // 0 - store only to 9 - means best compression
                    System.Console.WriteLine("Input Compression Level");
                    System.Console.WriteLine("0 - store only to 9 - means best compression");
                    int.TryParse(Console.ReadLine(), out int CompressionLevel);
                    OutputStream.SetLevel(CompressionLevel);

                    byte[] buffer = new byte[4096];

                    foreach (var dir in selectedDir.GetDirectories())
                    {
                        foreach (string file in dir.GetFiles().Select(f => f.FullName.ToString()))
                        {

                            // Using GetFileName makes the result compatible with XP
                            // as the resulting path is not absolute.
                            ZipEntry entry = new ZipEntry(Path.GetFileName(file));

                            // Setup the entry data as required.

                            // Crc and size are handled by the library for seakable streams
                            // so no need to do them here.

                            // Could also use the last write time or similar for the file.
                            entry.DateTime = DateTime.Now;
                            OutputStream.PutNextEntry(entry);

                            using (FileStream fs = File.OpenRead(file))
                            {

                                // Using a fixed size buffer here makes no noticeable difference for output
                                // but keeps a lid on memory usage.
                                int sourceBytes;

                                do
                                {
                                    sourceBytes = await fs.ReadAsync(buffer, 0, buffer.Length);
                                    await OutputStream.WriteAsync(buffer, 0, sourceBytes);
                                } while (sourceBytes > 0);
                            }
                        }
                    }


                    // Finish/Close arent needed strictly as the using statement does this automatically

                    // Finish is important to ensure trailing information for a Zip file is appended.  Without this
                    // the created file would be invalid.
                    OutputStream.Finish();

                    // Close is important to wrap things up and unlock the file.
                    OutputStream.Close();

                    Console.WriteLine("Files successfully compressed");
                }
            }
            catch (Exception ex)
            {
                // No need to rethrow the exception as for our purposes its handled.
                Console.WriteLine("Exception during processing {0}", ex);
            }
        }
    }
}