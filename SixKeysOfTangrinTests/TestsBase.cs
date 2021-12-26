namespace SixKeysOfTangrinTests;

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

        UseGameWithTangrinMap();

        game.Map.Initialise();
    }

    protected void UseGameWithTangrinMap()
    {
        game = new Game(
            inputDevice,
            outputDevice,
            randomGenerator,
            MapInstance(),
            suspense,
            player,
            inventory);
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

    // EN Energy
    // TI Time left
    // N 30 Number of Locations
    // N(30) locations (used to temporarily keep track of objects during creation)
    // H(3) Inventory
    // I(6) Key containers
    // J(6) Container content
    // G(30) Location items
    // P$(30) Location descriptions
    // C(30, 6) Cave exits
    // S$(30) location descriptions
}
