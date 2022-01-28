using System.Threading.Tasks;

public interface ISharpZipLibMethods
{
    Task<string[]> ListAllZipsInTestData();
    Task UnzipZipFile();
    Task<bool> RunProgram();
}