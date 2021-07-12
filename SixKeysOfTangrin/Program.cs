using Edvella.Devices.Console;
using Sentry;
using SixKeysOfTangrin.Effects;

namespace SixKeysOfTangrin
{
    public static class Program
    {
        static void Main(string[] args)
        {
            using (SentrySdk.Init(o =>
            {
                o.Dsn = "https://36dd72e698024b2d8ebef7414e4bea78@o883534.ingest.sentry.io/5836922";
                o.Debug = false;
                // Set traces_sample_rate to 1.0 to capture 100% of transactions for performance monitoring.
                // We recommend adjusting this value in production.
                o.TracesSampleRate = 1.0;
            }))
            {
                var inputDevice = new ConsoleInputDevice();
                var outputDevice = new ConsoleOutputDevice();
                var randomGenerator = new StandardRandomGenerator();
                var map = new TangrinMap(randomGenerator, outputDevice);
                var player = new Player();
                var suspense = new Suspense();

                Game game = new(
                    inputDevice,
                    outputDevice,
                    randomGenerator,
                    map,
                    suspense,
                    player,
                    new ThreeSlotInventory(
                        outputDevice, inputDevice, map, player, suspense));

                game.Start();
            }
        }
    }
}
