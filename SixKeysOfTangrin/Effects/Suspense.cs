namespace SixKeysOfTangrin.Effects;

public class Suspense : ISuspense
{
    public void Delay(int millisecods)
    {
        Thread.Sleep(millisecods);
    }
}
