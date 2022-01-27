using System.Threading.Tasks;

public interface ISharpZipLibMethods
{
    Task ListAllZipsInTestData();
    Task UnzipZipFile();
    Task<bool> RunProgram();
}