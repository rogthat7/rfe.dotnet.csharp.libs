using System.Threading.Tasks;

public interface ISharpZipLibMethods
{
    Task ListAllZipsInTestData();

    Task<bool> RunProgram();
}