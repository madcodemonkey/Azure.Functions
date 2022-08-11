namespace DotNetCoreNotIsolated.Services;

public class MyMathService : IMyMathService
{
    /// <summary>Adds to integers</summary>
    public int AddTwoIntegers(int first, int second)
    {
        return first + second;
    }
}