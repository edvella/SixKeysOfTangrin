namespace SixKeysOfTangrin
{
    public interface IRandomGenerator
    {
        int Next(int maxValue);
        int Next(int minValue, int maxValue);
        double NextDouble();
        double NextDouble(double maxValue);
    }
}