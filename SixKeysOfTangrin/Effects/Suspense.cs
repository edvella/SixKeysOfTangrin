using System.Threading.Tasks;

namespace SixKeysOfTangrin.Effects
{
    public class Suspense : ISuspense
    {
        public void Delay(int millisecods)
        {
            Task.Delay(1000); 
        }
    }
}
