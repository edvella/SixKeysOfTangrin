using Edvella.Devices;
using NSubstitute;
using SixKeysOfTangrin;
using SixKeysOfTangrin.Effects;

namespace SixKeysOfTangrinTests
{
    public class TestsBase
    {
        protected IInputDevice inputDevice;
        protected IOutputDevice outputDevice; 
        protected IRandomGenerator randomGenerator; 
        protected ISuspense suspense;
        protected IPlayer player;
        protected IInventory inventory;
        protected Game game;

        public TestsBase()
        {
            inputDevice = Substitute.For<IInputDevice>();
            outputDevice = Substitute.For<IOutputDevice>();
            randomGenerator = new StandardRandomGenerator();
            suspense = Substitute.For<ISuspense>();
            player = Substitute.For<IPlayer>();
            inventory = Substitute.For<IInventory>();

            inputDevice.ReadCommand().Returns(CommandPalette.End);

            game = new Game(
                inputDevice,
                outputDevice,
                randomGenerator,
                MapInstance(),
                suspense,
                player,
                inventory);

            game.Map.Initialise();
        }

        protected IMap MapInstance()
        {
            return new TangrinMap(
                new StandardRandomGenerator(), outputDevice);
        }

        protected void UseGameWithMockedMap()
        {
            game = new Game(
                inputDevice,
                outputDevice,
                randomGenerator,
                Substitute.For<IMap>(),
                suspense,
                player,
                inventory);
        }
    }
}