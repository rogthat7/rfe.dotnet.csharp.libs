namespace lib.dotnet.SharpZipLib
{
    /// <summary>
    /// 
    /// </summary>
    public class SharpZipLibMethodsTest
    {
        private readonly ISharpZipLibMethods _sharpZipLibMethods;

        public SharpZipLibMethodsTest(ISharpZipLibMethods sharpZipLibMethods) 
        {
            _sharpZipLibMethods = sharpZipLibMethods; 
        } 
        public async Task<bool> RunTask(int input)
        {
            
            await RunOperation(_sharpZipLibMethods, input);
            return true;
        }

        private static async Task RunOperation<T>(T operation, int input)
        where T : ISharpZipLibMethods 
        {
            await operation.ListAllZipsInTestData();
        }
    }
}