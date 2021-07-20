using Edvella.Devices;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SixKeysOfTangrin
{
    public class TangrinMap : IMap
    {
        public const int Locations = 30;

        public const int Exits = 6;

        public const int StartingLocation = 0;

        private const int connections = 33;

        private readonly int?[,] locations = new int?[Locations, 6];

        private readonly IRandomGenerator rnd;
        private readonly IOutputDevice outputDevice;

        private readonly Containers containers = new();
        public IEnumerable<int?> Containers() { return containers.ItemLocations(); }

        private readonly ContainerContent containerContent = new();
        public ContainerContent ContainerContent() { return containerContent; }

        public readonly ItemCollection itemCollection = new();
        public ItemCollection Items() { return itemCollection; }

        public int LocationCount => locations.GetLength(0);

        public int MaxExitsPerLocation => locations.GetLength(1);

        public int PlayerLocation { get; set; }

        public const string NextToARockText = "Next to a rock";
        public const string AgainstAWallText = "Against a wall";
        public const string OnTheGroundText = "On the ground";
        public const string ReflectingInYourTorchlightText = "Reflecting in your torchlight";
        public const string ExitsText = "You can now go : ";
        public const string InvalidDirectionText = "Can't go that way";
        public const string GotOutWithTreasureText = "You got out with the treasure.";

        public const string North = "north ";
        public const string East = "east ";
        public const string Up = "up ";
        public const string South = "south ";
        public const string West = "west ";
        public const string Down = "down ";

        public TangrinMap(
            IRandomGenerator randomGenerator,
            IOutputDevice outputDevice)
        {
            rnd = randomGenerator;
            this.outputDevice = outputDevice;
        }

        public void Initialise()
        {
            for (int i = 0; i < connections; i++)
                PlaceTwoWayConnection(
                    rnd.Next(Locations),
                    RandomExit(),
                    RandomDestination());

            AddAtLeastOneConnectionPerLocation();

            containers.ScatterAroundMap();
            containerContent.ScatterAroundMap();
            itemCollection.ScatterAroundMap();

            PlayerLocation = StartingLocation;
        }

        private int RandomExit()
        {
            return rnd.Next(Exits);
        }

        private int RandomDestination()
        {
            return rnd.Next(1, Locations);
        }

        public void PlaceTwoWayConnection(
            int x, int y, int destination, bool fixedLocation = false)
        {
            if (x < 0 || x >= Locations || y < 0 || y >= Exits)
                throw new ConnectionOutOfMapException();

            if (CanConnect(x, y, destination))
            {
                locations[x, y] = destination;
                locations[destination, OppositeDirection(y)] = x;
            }
            else
            {
                PlaceTwoWayConnection(
                    fixedLocation ? x : rnd.Next(Locations),
                    RandomExit(),
                    RandomDestination(),
                    fixedLocation);
            }
        }

        private bool CanConnect(int x, int y, int destination)
        {
            return !DestinationLocation(x, y).HasValue &&
                !DestinationLocation(destination, OppositeDirection(y)).HasValue &&
                x != destination;
        }

        public int? DestinationLocation(int x, int y)
        {
            return locations[x, y];
        }

        private void AddAtLeastOneConnectionPerLocation()
        {
            for (var x = 0; x < Locations; x++)
            {
                var caveConnections = 0;
                for (var y = 0; y < Exits; y++)
                    if (DestinationLocation(x, y).HasValue) caveConnections++;

                if (caveConnections == 0)
                    PlaceTwoWayConnection(
                        x,
                        RandomExit(),
                        RandomDestination(),
                        true);
            }
        }

        public static int OppositeDirection(int y)
        {
            if (y < 3) return y + 3;

            return y - 3;
        }

        public static readonly string[] caveDescriptions =
        {
            "You are under a cliff, near an old house. A cave faces you.",
            "Ahead of you lies an underground stream, but you can cross it.",
            "You are in a low chamber and must crawl on hands and knees.",
            "You are in a small cave with flourescent green walls.",
            "You come across the home of Gor the giant, but he is out now.",
            "You are in a man-made room with flowered wall paper.",
            "You are in a featureless cave & hear the sound of running water.",
            "The walls open out into a very wide room.",
            "In this narrow room, with a low ceiling sits a chunky wooden tab.",
            "Ahead lies a prehistoric place with bones strewn everywhere.",
            "You pass some mysterious drawings, chalked centuries ago.",
            "You walk across a rotting bridge spanning a bottomless chasm.",
            "You approach an old stone statue that points back the way you came.",
            "You now come into a small cave, littered by hundreds of mk14s.",
            "You enter a broad plain, with a glossy, metallic floor.",
            "You are in a rocky chamber - the smell of decay hangs in the air.",
            "You pass through a broken door into a 17th century library.",
            "You pass through a doorway into a disused laboratory.",
            "You enter what was a nursery - a rocking horse stares at you.",
            "You are in a wine cellar, all the bottles are smashed.",
            "Ahead lies a small office with papers strewn everywhere.",
            "You are in a cold, damp cave and shiver involuntarily.",
            "Ahead lies a rock formation like that of a honeycomb.",
            "Water drips from the ceiling of this cave. Better hurry or get wet.",
            "Now you have to dodge round some rather sharp stalagmites.",
            "You walk across some gravel, trying hard not to stumble.",
            "You cross some rough terrain, avoiding the sharp-edged walls.",
            "Now you are in an oval cave with smooth walls.",
            "You're walking on stoney ground that makes walking uncomfortable.",
            "You pass through an old school room, still with desks."
        };

        public readonly string[] items =
        {
            "a small red box",
            "a wooden cupboard",
            "a large briefcase",
            "a wooden trunk",
            "an antique sideboard",
            "a large, robust, treasure chest",
            "a small copper box",
            "a tin of food",
            "a skeleton of a man - perhaps the last treasure hunter",
            "a dead rat",
            "a silver key",
            "a golden key",
            "a bronze key",
            "a nickel key, green with age",
            "a shiny, pointed key",
            "a key with the initials 'J.F' scratched on",
            "a length of rope",
            "a box of uncompleted rubik cubes",
            "a pile of old 78's",
            "a tin opener",
            "a French dictionary",
            "a tourist's guide to the West",
            "a 1946 Daily Mirror",
            "a World War I helmet",
            "a long knife",
            "a comfy chair",
            "a pair of dentures",
            "a rock from STAR TREK",
            "THE TREASURE", //"a pile of radio components",
            "a gun",
            "NOTHING", //"a packet of playing cards",
            "A huge shell"
        };

        public void VisibleItem()
        {
            if (ItemInCurrentLocation() != ItemCollection.Nothing)
            {
                if (rnd.NextDouble(1) > .95)
                {
                    for (var i = 0; i < LocationCount; i++)
                    {
                        if (Items().ItemLocations().ElementAt(i) == ItemCollection.Nothing)
                        {
                            Items().UpdateItem(PlayerLocation, 6);
                            break;
                        }
                    }
                }

                outputDevice.ShowMessage(
                    $"{VisibleItemLocation()} is {ItemInCurrentLocationDescription()}.");
            }
        }

        public string ItemInCurrentLocationDescription()
        {
            return items[ItemInCurrentLocation()];
        }

        public string ItemDescription(int index)
        {
            return items[index];
        }

        public int ItemInCurrentLocation()
        {
            return Items().ItemLocations().ElementAt(PlayerLocation).Value;
        }

        private string VisibleItemLocation()
        {
            if (rnd.NextDouble(1.0) < .3) return NextToARockText;
            if (rnd.NextDouble(2.0) < .3) return AgainstAWallText;
            if (rnd.NextDouble(2.0) < .3) return OnTheGroundText;

            return ReflectingInYourTorchlightText;
        }

        public string LookCommand()
        {
            return caveDescriptions[PlayerLocation];
        }

        public string CurrentExits()
        {
            var exits = new StringBuilder();
            if (locations[PlayerLocation, 0].HasValue) exits.Append(North);
            if (locations[PlayerLocation, 1].HasValue) exits.Append(East);
            if (locations[PlayerLocation, 2].HasValue) exits.Append(Up);
            if (locations[PlayerLocation, 3].HasValue) exits.Append(South);
            if (locations[PlayerLocation, 4].HasValue) exits.Append(West);
            if (locations[PlayerLocation, 5].HasValue) exits.Append(Down);

            return $"{ExitsText}{exits}\n";
        }

        public bool GoNorth()
        {
            return Go(0);
        }

        public bool GoEast()
        {
            return Go(1);
        }

        public bool GoUp()
        {
            return Go(2);
        }

        public bool GoSouth()
        {
            return Go(3);
        }

        public bool GoWest()
        {
            return Go(4);
        }

        public bool GoDown()
        {
            return Go(5);
        }

        private bool Go(int direction)
        {
            if (DestinationLocation(PlayerLocation, direction) != null)
            {
                PlayerLocation = DestinationLocation(PlayerLocation, direction).Value;
                return true;
            }
            else
            {
                outputDevice.ShowMessage(InvalidDirectionText);
                return false;
            }
        }

        public void RemoveItemFromCurrentLocation()
        {
            Items().UpdateItem(PlayerLocation, ItemCollection.Nothing);
        }

        public void AddItemToCurrentLocation(int item)
        {
            Items().UpdateItem(PlayerLocation, item);
        }
    }
}
