namespace SixKeysOfTangrin;

public class StandardRandomGenerator : IRandomGenerator
{
    private readonly Random rnd = new(DateTime.Now.Second);

    public int Next(int maxValue)
    {
        return rnd.Next(maxValue);
    }

    public int Next(int minValue, int maxValue)
    {
        return rnd.Next(minValue, maxValue);
    }

    public double NextDouble()
    {
        return rnd.NextDouble();
    }

    public double NextDouble(double maxValue)
    {
        return NextDouble() * maxValue;
    }
}
