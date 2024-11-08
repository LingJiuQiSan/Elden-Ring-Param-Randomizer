using MathNet.Numerics.Distributions;

namespace Elden_Ring_Param_Randomizer;

public abstract class Utils
{
    public static double GetExponentiallyDistributedRandom(int maxValue)
    {
        var exponential = new Exponential(1.0 / maxValue);
        return exponential.Sample();
    }
}