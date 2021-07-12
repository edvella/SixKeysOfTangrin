using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SixKeysOfTangrin;
using System;
using System.Linq;

namespace SixKeysOfTangrinTests
{
    [TestClass]
    public class MapTests : TestsBase
    {
        private TangrinMap map;
        private IRandomGenerator mockRandom;

        [TestInitialize]
        public void Setup()
        {
            map = new TangrinMap(randomGenerator, outputDevice);
            mockRandom = Substitute.For<IRandomGenerator>();
        }

        [TestMethod]
        public void MapContainsThirtyLocations()
        {
            map.LocationCount.Should().Be(TangrinMap.Locations);
        }

        [TestMethod]
        public void LocationsCanHaveSixExits()
        {
            map.MaxExitsPerLocation.Should().Be(6);
        }

        [TestMethod]
        public void CanAddConnectionToFirstLocation()
        {
            map.PlaceTwoWayConnection(0, 0, 2);
            map.DestinationLocation(0, 0).Should().Be(2);
        }

        [TestMethod]
        public void CanAddConnectionToLastLocation()
        {
            map.PlaceTwoWayConnection(29, 0, 2);
            map.DestinationLocation(29, 0).Should().Be(2);
        }

        [TestMethod]
        public void CannotPlaceConnectionOutsideOfLocationLowerBound()
        {
            map.Invoking(_ => _.PlaceTwoWayConnection(-1, 0, 2))
                .Should().Throw<ConnectionOutOfMapException>();
        }

        [TestMethod]
        public void CannotPlaceConnectionOutsideOfLocationUpperBound()
        {
            map.Invoking(_ => _.PlaceTwoWayConnection(TangrinMap.Locations, 0, 2))
                .Should().Throw<ConnectionOutOfMapException>();
        }

        [TestMethod]
        public void CannotPlaceConnectionOutsideOfLocationExitLowerBound()
        {
            map.Invoking(_ => _.PlaceTwoWayConnection(0, -1, 2))
                .Should().Throw<ConnectionOutOfMapException>();
        }

        [TestMethod]
        public void CannotPlaceConnectionOutsideOfLocationExitUpperBound()
        {
            map.Invoking(_ => _.PlaceTwoWayConnection(0, 6, 2))
                .Should().Throw<ConnectionOutOfMapException>();
        }

        [TestMethod]
        public void TryingToPlaceConnectionInOccupiedSpotPlacesItRandomly()
        {
            map.PlaceTwoWayConnection(0, 0, 2);
            map.PlaceTwoWayConnection(0, 0, 3);

            var connections = 0;
            for (var x = 0; x < TangrinMap.Locations; x++)
                for (var y = 0; y < TangrinMap.Exits; y++)
                    if (map.DestinationLocation(x, y).HasValue)
                        connections++;

            connections.Should().Be(4);
            map.DestinationLocation(0, 0).Should().Be(2);
        }

        [TestMethod]
        public void MapInitialisaionPlacesAtLeastThirtyTreeConnection()
        {
            map.Initialise();
            var placedItems = 0;
            for (var x = 0; x < TangrinMap.Locations; x++)
                for (var y = 0; y < TangrinMap.Exits; y++)
                    if (map.DestinationLocation(x, y) != 0) placedItems++;
            placedItems.Should().BeGreaterOrEqualTo(33);
        }

        [TestMethod]
        public void EachLocationHasAtLeaseOneConnection()
        {
            map.Initialise();
            for (var x = 0; x < TangrinMap.Locations; x++)
            {
                var LocationConnections = 0;
                for (var y = 0; y < TangrinMap.Exits; y++)
                {
                    if (map.DestinationLocation(x, y).HasValue)
                        LocationConnections++;
                }

                LocationConnections.Should().BeGreaterThan(0);
            }
        }

        [TestMethod]
        public void EachLocationConnectionShouldWorkBothWays()
        {
            map.Initialise();
            for (var x = 0; x < TangrinMap.Locations; x++)
            {
                for (var y = 0; y < TangrinMap.Exits; y++)
                {
                    var destinationLocation = map.DestinationLocation(x, y);
                    if (destinationLocation.HasValue)
                    {
                        map.DestinationLocation(
                            destinationLocation.Value, TangrinMap.OppositeDirection(y)
                        ).Should().Be(x);
                    }
                }
            }
        }

        [TestMethod]
        public void LocationCannotBeConnectedWithItself()
        {
            map.PlaceTwoWayConnection(0, 0, 0);
            map.DestinationLocation(0, 0).Should().NotBe(0);
        }

        [TestMethod]
        public void OnlyOneOfEachItemTypeCanBePlacedInAnyLocation()
        {
            var c = new ItemCollection(6, 0, 7);
            c.PlaceItem(1, 1);
            c.PlaceItem(2, 1);
            c.ItemLocations().ElementAt(1).Should().Be(1);
        }

        [TestMethod]
        public void SixKeysAreMatchedUpWithLockedContainers()
        {
            map.Initialise();
            foreach (int? keyLocation in map.Containers())
            {
                keyLocation.Should().BeInRange(0, 6);
            }
        }

        [TestMethod]
        public void SixKeysArePlacedInsideContainers()
        {
            map.Initialise();
            var keyLocations = map.ContainerContent().ItemLocations();
            foreach (int? keyLocation in keyLocations)
            {
                if (!keyLocation.Equals(keyLocations.ElementAt(5)))
                    keyLocation.Should().BeInRange(10, 16);
            }
        }

        [TestMethod]
        public void TreasureIsAlwaysInLastContainer()
        {
            map.Initialise();
            map.ContainerContent().ItemLocations().ElementAt(5).Should().Be(ItemCollection.Treasure);
        }

        [TestMethod]
        public void AllItemsAreScatteredAroundTheMap()
        {
            map.Initialise();
            map.Items().ItemLocations().Count(_ => !_.HasValue).Should().BeLessOrEqualTo(1);
        }

        [TestMethod]
        public void TreasureCannotBeFoundLyingAround()
        {
            map.Initialise();
            foreach (int? itemLocation in map.Items().ItemLocations())
            {
                itemLocation.Should().NotBe(ItemCollection.Treasure);
            }
        }

        [TestMethod]
        public void PlayerStartsUnderCliffNearHouse()
        {
            map.Initialise();
            map.PlayerLocation.Should().Be(TangrinMap.StartingLocation);
        }

        [TestMethod]
        public void CanDisplayTheLocationDescription()
        {
            map.PlayerLocation = 20;
            map.LookCommand().Should().Be(TangrinMap.caveDescriptions[20]);
        }

        [TestMethod]
        public void ItemsCanRandomlyBeNextToARock()
        {
            InitialiseMap();
            mockRandom.NextDouble(1.0).Returns(0.29);
            map.VisibleItem().Should().StartWith(TangrinMap.NextToARockText);
        }

        [TestMethod]
        public void ItemsCanRandomlyBeAgainstAWall()
        {
            InitialiseMap();
            mockRandom.NextDouble(1.0).Returns(0.3);
            mockRandom.NextDouble(2.0).Returns(0.29);
            map.VisibleItem().Should().StartWith(TangrinMap.AgainstAWallText);
        }

        [TestMethod]
        public void ItemsCanRandomlyBeOnTheGround()
        {
            InitialiseMap();
            mockRandom.NextDouble(1.0).Returns(0.3);
            mockRandom.NextDouble(2.0).Returns(0.3, 0.29);
            map.VisibleItem().Should().StartWith(TangrinMap.OnTheGroundText);
        }

        [TestMethod]
        public void ItemsCanRandomlyBeReflectingInYourTorchlight()
        {
            InitialiseMap();
            mockRandom.NextDouble(1.0).Returns(0.3);
            mockRandom.NextDouble(2.0).Returns(0.3, 0.3);
            map.VisibleItem().Should().StartWith(TangrinMap.ReflectingInYourTorchlightText);
        }

        private void InitialiseMap()
        {
            mockRandom.Next(30).Returns(_ => new Random().Next(30));
            mockRandom.Next(6).Returns(_ => new Random().Next(6));
            mockRandom.Next(1, 30).Returns(_ => new Random().Next(1, 30));
            map = new TangrinMap(mockRandom, outputDevice);
            map.Initialise();
        }

        [TestMethod]
        public void CanShowPossibleExits()
        {
            map.CurrentExits().Should().StartWith(TangrinMap.ExitsText);
        }

        [TestMethod]
        public void ShowsIfPlayerCanGoNorth()
        {
            map.PlaceTwoWayConnection(10, 0, 11);
            map.PlayerLocation = 10;
            map.CurrentExits().Should().Contain(TangrinMap.North);
        }

        [TestMethod]
        public void ShowsIfPlayerCanGoEast()
        {
            map.PlaceTwoWayConnection(10, 1, 11);
            map.PlayerLocation = 10;
            map.CurrentExits().Should().Contain(TangrinMap.East);
        }

        [TestMethod]
        public void ShowsIfPlayerCanGoUp()
        {
            map.PlaceTwoWayConnection(10, 2, 11);
            map.PlayerLocation = 10;
            map.CurrentExits().Should().Contain(TangrinMap.Up);
        }

        [TestMethod]
        public void ShowsIfPlayerCanGoSouth()
        {
            map.PlaceTwoWayConnection(10, 3, 11);
            map.PlayerLocation = 10;
            map.CurrentExits().Should().Contain(TangrinMap.South);
        }

        [TestMethod]
        public void ShowsIfPlayerCanGoWest()
        {
            map.PlaceTwoWayConnection(10, 4, 11);
            map.PlayerLocation = 10;
            map.CurrentExits().Should().Contain(TangrinMap.West);
        }

        [TestMethod]
        public void ShowsIfPlayerCanGoDown()
        {
            map.PlaceTwoWayConnection(10, 5, 11);
            map.PlayerLocation = 10;
            map.CurrentExits().Should().Contain(TangrinMap.Down);
        }

        [TestMethod]
        public void NorthCommandTakesPlayerNorth()
        {
            map.PlaceTwoWayConnection(5, 0, 10);
            map.PlayerLocation = 5;
            map.GoNorth();
            map.PlayerLocation.Should().Be(10);
        }

        [TestMethod]
        public void EastCommandTakesPlayerEast()
        {
            map.PlaceTwoWayConnection(5, 1, 10);
            map.PlayerLocation = 5;
            map.GoEast();
            map.PlayerLocation.Should().Be(10);
        }

        [TestMethod]
        public void UpCommandTakesPlayerUp()
        {
            map.PlaceTwoWayConnection(5, 2, 10);
            map.PlayerLocation = 5;
            map.GoUp();
            map.PlayerLocation.Should().Be(10);
        }

        [TestMethod]
        public void SouthCommandTakesPlayerSouth()
        {
            map.PlaceTwoWayConnection(5, 3, 10);
            map.PlayerLocation = 5;
            map.GoSouth();
            map.PlayerLocation.Should().Be(10);
        }

        [TestMethod]
        public void WestCommandTakesPlayerWest()
        {
            map.PlaceTwoWayConnection(5, 4, 10);
            map.PlayerLocation = 5;
            map.GoWest();
            map.PlayerLocation.Should().Be(10);
        }

        [TestMethod]
        public void DownCommandTakesPlayerDown()
        {
            map.PlaceTwoWayConnection(5, 5, 10);
            map.PlayerLocation = 5;
            map.GoDown();
            map.PlayerLocation.Should().Be(10);
        }

        [TestMethod]
        public void StaysInTheSamePlaceWhenDirectionIsNotAllowed()
        {
            map.PlaceTwoWayConnection(5, 5, 10);
            map.PlayerLocation = 5;
            map.GoUp();
            map.PlayerLocation.Should().Be(5);
        }

        [TestMethod]
        public void ShowsMessageWhenDirectionIsNotAllowed()
        {
            map.PlaceTwoWayConnection(5, 5, 10);
            map.PlayerLocation = 5;
            map.GoUp();
            outputDevice.Received(1).ShowMessage(TangrinMap.InvalidDirectionText);
        }

        [TestMethod]
        public void CanRemoveItemsFromTheMap()
        {
            map.PlayerLocation = 6;
            map.RemoveItemFromCurrentLocation();
            map.ItemInCurrentLocation().Should().Be(ItemCollection.Nothing);
        }

        [TestMethod]
        public void CanAddItemsToTheMap()
        {
            map.PlayerLocation = 6;
            map.AddItemToCurrentLocation(12);
            map.ItemInCurrentLocation().Should().Be(12);
        }

        // WIP: 2726

        // H(1..3) set to 0
        // visible item suspence not in correct spot
        // skipped 2019
        // Tangrin gosub 5700 check on line 2474
        // 4000 - 4060 done
        // 9000 - 9090 done, but end game when player dies

        // 2515 - 2740: Open        done
        // 2805 - 2880: Pick up     done
        // 2910 - 2992: Swap        done
        // 3000: End                done
        // 3110 - 3180: Dump        done
        // 3310 - 3500: Movement  3300 - 3410 done

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
}
