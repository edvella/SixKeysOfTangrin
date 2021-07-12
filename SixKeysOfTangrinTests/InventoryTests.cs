using Edvella.Devices;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SixKeysOfTangrin;
using System.Linq;

namespace SixKeysOfTangrinTests
{
    [TestClass]
    public class InventoryTests : TestsBase
    {
        private ThreeSlotInventory threeSlotInventory;

        [TestInitialize]
        public void Init()
        {
            UseGameWithMockedMap();
            threeSlotInventory = new ThreeSlotInventory(
                outputDevice, inputDevice, game.Map, player, suspense);

            game.Map.ItemDescription(0).Returns("a small red box");
            game.Map.ItemDescription(10).Returns("a silver key");
            game.Map.ItemDescription(11).Returns("a golden key");
            game.Map.ItemDescription(20).Returns("a French dictionary");
            game.Map.ItemDescription(16).Returns("a length of rope");
            game.Map.ItemDescription(26).Returns("a pair of dentures");
        }

        [TestMethod]
        public void ShowsMessageWhenThereIsNothingToPickUp()
        {
            game.Map.ItemInCurrentLocation().Returns(ItemCollection.Nothing);
            threeSlotInventory.PickUp();
            outputDevice.Received(1)
                .ShowMessage(ThreeSlotInventory.NothingToPickupText);
        }

        [TestMethod]
        public void DoesNotLoseTurnWhenPickingItemsUp()
        {
            threeSlotInventory.PickUp().Should().BeFalse();
        }

        [TestMethod]
        public void PlayerCannotPickContainersUp()
        {
            game.Map.ItemInCurrentLocation().Returns(0, 6);
            threeSlotInventory.PickUp();
            outputDevice.Received(1)
                .ShowMessage(ThreeSlotInventory.MagicalForceText);
        }

        [TestMethod]
        public void PickUpDisplaysItemsInInventory()
        {
            game.Map.ItemInCurrentLocation().Returns(26);
            threeSlotInventory.Content[0] = 11;
            threeSlotInventory.Content[1] = 20;
            threeSlotInventory.Content[2] = 16;
            threeSlotInventory.PickUp();
            outputDevice.Received(1).ShowMessage("You hold the following");
            outputDevice.Received(1).ShowMessage("1. a golden key");
            outputDevice.Received(1).ShowMessage("2. a French dictionary");
            outputDevice.Received(1).ShowMessage("3. a length of rope");
        }

        [TestMethod]
        public void DoesNotShowAnyMessageIfInventoryIsEmpty()
        {
            game.Map.ItemInCurrentLocation().Returns(26);
            threeSlotInventory.PickUp();
            outputDevice.DidNotReceive().ShowMessage("You hold the following");
        }

        [TestMethod]
        public void PickupFailsIfInventoryIsFull()
        {
            game.Map.ItemInCurrentLocation().Returns(26);
            threeSlotInventory.Content[0] = 11;
            threeSlotInventory.Content[1] = 20;
            threeSlotInventory.Content[2] = 16;
            threeSlotInventory.PickUp();
            outputDevice.Received(1).ShowMessage(ThreeSlotInventory.InventoryFullText);
        }

        [TestMethod]
        public void ConfirmsWhenItemIsPickedUp()
        {
            game.Map.ItemInCurrentLocation().Returns(26);
            threeSlotInventory.PickUp();
            outputDevice.Received(1).ShowMessage("OK I've picked up the pair of dentures.");
        }

        [TestMethod]
        public void CanPickOneItemUpAndAddToInventory()
        {
            game.Map.ItemInCurrentLocation().Returns(26);
            threeSlotInventory.PickUp();
            threeSlotInventory.Content[0].Should().Be(26);
        }

        [TestMethod]
        public void CanPickTwoItemsUpAndAddToInventory()
        {
            game.Map.ItemInCurrentLocation().Returns(16);
            threeSlotInventory.PickUp();
            game.Map.ItemInCurrentLocation().Returns(26);
            threeSlotInventory.PickUp();

            threeSlotInventory.Content[0].Should().Be(16);
            threeSlotInventory.Content[1].Should().Be(26);
        }

        [TestMethod]
        public void CanPickThreeItemsUpAndAddToInventory()
        {
            game.Map.ItemInCurrentLocation().Returns(16);
            threeSlotInventory.PickUp();
            game.Map.ItemInCurrentLocation().Returns(11);
            threeSlotInventory.PickUp();
            game.Map.ItemInCurrentLocation().Returns(26);
            threeSlotInventory.PickUp();

            threeSlotInventory.Content[0].Should().Be(16);
            threeSlotInventory.Content[1].Should().Be(11);
            threeSlotInventory.Content[2].Should().Be(26);
        }

        [TestMethod]
        public void PickedUpItemsAreRemovedFromTheMap()
        {
            game.Map.ItemInCurrentLocation().Returns(26);
            threeSlotInventory.PickUp();
            game.Map.Received(1).RemoveItemFromCurrentLocation();
        }

        [TestMethod]
        public void CannotDumpItemIfThereIsAnItemAlreadyInTheCurrentLocation()
        {
            game.Map.ItemInCurrentLocation().Returns(26);
            threeSlotInventory.Dump();
            outputDevice.Received(1).ShowMessage(ThreeSlotInventory.ItemAlredyPresentText);
        }

        [TestMethod]
        public void CannotDumpItemIfInventoryIsEmpty()
        {
            game.Map.ItemInCurrentLocation().Returns(ItemCollection.Nothing);
            threeSlotInventory.Dump();
            outputDevice.Received(1).ShowMessage(ThreeSlotInventory.InventoryEmptyText);
        }

        [TestMethod]
        public void DumpDisplaysItemsInInventoryForThePlayerToChoose()
        {
            game.Map.ItemInCurrentLocation().Returns(ItemCollection.Nothing);
            threeSlotInventory.Content[0] = 11;
            threeSlotInventory.Content[1] = 20;
            threeSlotInventory.Content[2] = 16;
            inputDevice.ChooseListItem(Arg.Any<string>()).Returns(1);
            threeSlotInventory.Dump();
            outputDevice.Received(1).ShowMessage("You hold the following");
            outputDevice.Received(1).ShowMessage("1. a golden key");
            outputDevice.Received(1).ShowMessage("2. a French dictionary");
            outputDevice.Received(1).ShowMessage("3. a length of rope");
        }

        [TestMethod]
        public void DumpCommandPromptsPlayerToChooseItemToDrop()
        {
            game.Map.ItemInCurrentLocation().Returns(ItemCollection.Nothing);
            threeSlotInventory.Content[0] = 11;
            inputDevice.ChooseListItem(Arg.Any<string>()).Returns(1);
            threeSlotInventory.Dump();
            inputDevice.Received(1).ChooseListItem(ThreeSlotInventory.InventoryChoiceText);
        }

        [TestMethod]
        public void DumpingItemPlacesItInCurrentLocation()
        {
            game.Map.ItemInCurrentLocation().Returns(ItemCollection.Nothing);
            threeSlotInventory.Content[0] = 11;
            inputDevice.ChooseListItem(Arg.Any<string>()).Returns(1);
            threeSlotInventory.Dump();
            game.Map.Received(1).AddItemToCurrentLocation(11);
        }

        [TestMethod]
        public void DoesNotLoseTurnWhenDumpingItems()
        {
            threeSlotInventory.Dump().Should().BeFalse();
        }

        [TestMethod]
        public void DumpPromptsItemChoiceAgainIfIncorrectChoiceNumberIsSelected()
        {
            game.Map.ItemInCurrentLocation().Returns(ItemCollection.Nothing);
            threeSlotInventory.Content[0] = 11;
            inputDevice.ChooseListItem(Arg.Any<string>()).Returns(0, 1);
            threeSlotInventory.Dump();
            inputDevice.Received(2).ChooseListItem(ThreeSlotInventory.InventoryChoiceText);
        }

        [TestMethod]
        public void DumpPromptsItemChoiceAgainIfEmptySlotIsChosen()
        {
            game.Map.ItemInCurrentLocation().Returns(ItemCollection.Nothing);
            threeSlotInventory.Content[2] = 11;
            inputDevice.ChooseListItem(Arg.Any<string>()).Returns(2, 3);
            threeSlotInventory.Dump();
            inputDevice.Received(2).ChooseListItem(ThreeSlotInventory.InventoryChoiceText);
        }

        [TestMethod]
        public void ItemIsRemovedFromInventoryWhenDumped()
        {
            game.Map.ItemInCurrentLocation().Returns(ItemCollection.Nothing);
            threeSlotInventory.Content[2] = 11;
            inputDevice.ChooseListItem(Arg.Any<string>()).Returns(3);
            threeSlotInventory.Dump();
            threeSlotInventory.Content[2].Should().BeNull();
        }

        [TestMethod]
        public void DoesNotLoseTurnWhenSwappingItems()
        {
            threeSlotInventory.Swap().Should().BeFalse();
        }

        [TestMethod]
        public void CannotSwapHeavyObjects()
        {
            game.Map.ItemInCurrentLocation().Returns(5);
            threeSlotInventory.Swap();
            outputDevice.Received(1).ShowMessage(ThreeSlotInventory.ObjectTooHeavyText);
        }

        [TestMethod]
        public void DisplaysItemsInInventoryForThePlayerToSwap()
        {
            game.Map.ItemInCurrentLocation().Returns(ItemCollection.Nothing);
            threeSlotInventory.Content[0] = 11;
            threeSlotInventory.Content[1] = 20;
            threeSlotInventory.Content[2] = 16;
            inputDevice.ChooseListItem(Arg.Any<string>()).Returns(1);
            threeSlotInventory.Swap();
            outputDevice.Received(1).ShowMessage("You hold the following");
            outputDevice.Received(1).ShowMessage("1. a golden key");
            outputDevice.Received(1).ShowMessage("2. a French dictionary");
            outputDevice.Received(1).ShowMessage("3. a length of rope");
        }

        [TestMethod]
        public void SwapAttemptShowsErrorIfInventoryIsEmpty()
        {
            game.Map.ItemInCurrentLocation().Returns(11);
            threeSlotInventory.Swap();
            outputDevice.Received(1).ShowMessage(ThreeSlotInventory.NothingToSwapText);
        }

        [TestMethod]
        public void SwapCommandPromptsPlayerToChooseItemToRemoveFromInventory()
        {
            game.Map.ItemInCurrentLocation().Returns(ItemCollection.Nothing);
            threeSlotInventory.Content[0] = 11;
            inputDevice.ChooseListItem(Arg.Any<string>()).Returns(1);
            threeSlotInventory.Swap();
            inputDevice.Received(1).ChooseListItem(ThreeSlotInventory.SwapItemSelectionText);
        }

        [TestMethod]
        public void SwapPromptsItemChoiceAgainIfIncorrectChoiceNumberIsSelected()
        {
            game.Map.ItemInCurrentLocation().Returns(ItemCollection.Nothing);
            threeSlotInventory.Content[0] = 11;
            inputDevice.ChooseListItem(Arg.Any<string>()).Returns(0, 1);
            threeSlotInventory.Swap();
            inputDevice.Received(2).ChooseListItem(ThreeSlotInventory.SwapItemSelectionText);
        }

        [TestMethod]
        public void SwapPromptsItemChoiceAgainIfEmptySlotIsChosen()
        {
            game.Map.ItemInCurrentLocation().Returns(ItemCollection.Nothing);
            threeSlotInventory.Content[2] = 11;
            inputDevice.ChooseListItem(Arg.Any<string>()).Returns(2, 3);
            threeSlotInventory.Swap();
            inputDevice.Received(2).ChooseListItem(ThreeSlotInventory.SwapItemSelectionText);
        }

        [TestMethod]
        public void ChosenInventoryItemIsPlacedInCurrentLocationWhenSwapping()
        {
            game.Map.ItemInCurrentLocation().Returns(19);
            threeSlotInventory.Content[0] = 11;
            inputDevice.ChooseListItem(Arg.Any<string>()).Returns(1);
            threeSlotInventory.Swap();
            game.Map.Received(1).AddItemToCurrentLocation(11);
        }

        [TestMethod]
        public void LocationItemIsPlacedInInventoryWhenSwapping()
        {
            game.Map.ItemInCurrentLocation().Returns(19);
            threeSlotInventory.Content[0] = 11;
            inputDevice.ChooseListItem(Arg.Any<string>()).Returns(1);
            threeSlotInventory.Swap();
            threeSlotInventory.Content[0].Should().Be(19);
        }

        [TestMethod]
        public void SwappingWithAnEmptyLocationIsAllowed()
        {
            game.Map.ItemInCurrentLocation().Returns(ItemCollection.Nothing);
            threeSlotInventory.Content[0] = 11;
            inputDevice.ChooseListItem(Arg.Any<string>()).Returns(1);
            threeSlotInventory.Swap();
            threeSlotInventory.Content[0].Should().Be(ItemCollection.Nothing);
            game.Map.Received(1).AddItemToCurrentLocation(11);
        }

        [TestMethod]
        public void DisplaysConfirmationMessageWhenSwapSucceeds()
        {
            game.Map.ItemInCurrentLocation().Returns(11);
            threeSlotInventory.Content[0] = 19;
            inputDevice.ChooseListItem(Arg.Any<string>()).Returns(1);
            threeSlotInventory.Swap();
            outputDevice.Received(1).ShowMessage("OK I've now got a golden key");
        }

        [TestMethod]
        public void OnlyContainersCanBeOpened()
        {
            game.Map.ItemInCurrentLocationDescription().Returns(
                "a skeleton of a man - perhaps the last treasure hunter");
            game.Map.ItemInCurrentLocation().Returns(8);
            threeSlotInventory.Open();
            outputDevice.Received(1).ShowMessage(
                "How can you open a skeleton of a man - perhaps the last treasure hunter?");
        }

        [TestMethod]
        public void CanOpenTinOfFoodWithTinOpener()
        {
            game.Map.ItemInCurrentLocation().Returns(ItemCollection.TinOfFood);
            threeSlotInventory.Content[0] = ItemCollection.TinOpener;
            threeSlotInventory.Open();
            outputDevice.Received(1).ShowMessage(ThreeSlotInventory.EatFoodText);
        }

        [TestMethod]
        public void EatingFoodRestoresEnergy()
        {
            game.Map.ItemInCurrentLocation().Returns(ItemCollection.TinOfFood);
            threeSlotInventory.Content[0] = ItemCollection.TinOpener;
            threeSlotInventory.Open();
            game.Player.Received(1).Restore(60);
        }

        [TestMethod]
        public void OpeningTinRemovesItFromTheMap()
        {
            game.Map.ItemInCurrentLocation().Returns(ItemCollection.TinOfFood);
            threeSlotInventory.Content[0] = ItemCollection.TinOpener;
            threeSlotInventory.Open();
            game.Map.Received(1).RemoveItemFromCurrentLocation();
        }

        [TestMethod]
        public void CannotOpenTinOfFoodWithoutTinOpener()
        {
            game.Map.ItemInCurrentLocation().Returns(ItemCollection.TinOfFood);
            threeSlotInventory.Open();
            outputDevice.Received(1).ShowMessage(ThreeSlotInventory.NoTinOpenerText);
        }

        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataTestMethod]
        public void CanOpenContainersWithMatchingKey(int slot)
        {
            game.Map.ItemInCurrentLocation().Returns(0);
            threeSlotInventory.Content[slot] = 10;
            game.Map.Containers().Returns(new int?[] { 0, 1, 2, 3, 4, 5, 6 });
            var keys = new ContainerContent();
            keys.PlaceItem(0, 11);
            game.Map.ContainerContent().Returns(keys);
            threeSlotInventory.Open();
            outputDevice.Received(1).ShowMessage(
                "Yes! You have the correct key.\nThe silver key fits.\nRight let's open it...");
        }

        [TestMethod]
        public void CannotOpenContainersWithoutMatchingKey()
        {
            game.Map.ItemInCurrentLocation().Returns(0);
            threeSlotInventory.Content[0] = 11;
            game.Map.Containers().Returns(new int?[] { 0, 1, 2, 3, 4, 5, 6 });
            threeSlotInventory.Open();
            outputDevice.Received(1)
                .ShowMessage("Sorry you don't have the key for the small red box.");
        }

        [TestMethod]
        public void RevealsTheContentOfAnOpenedContainer()
        {
            game.Map.ItemInCurrentLocation().Returns(0);
            threeSlotInventory.Content[0] = 10;
            game.Map.Containers().Returns(new int?[] { 0, 1, 2, 3, 4, 5, 6 });
            var keys = new ContainerContent();
            keys.PlaceItem(0, 11);
            game.Map.ContainerContent().Returns(keys);
            threeSlotInventory.Open();
            outputDevice.Received(1)
                .ShowMessage("Inside is a golden key");
        }

        [TestMethod]
        public void PromptsPlayerForActionAboutOpenedContainerContent()
        {
            game.Map.ItemInCurrentLocation().Returns(0);
            threeSlotInventory.Content[0] = 10;
            game.Map.Containers().Returns(new int?[] { 0, 1, 2, 3, 4, 5, 6 });
            var keys = new ContainerContent();
            keys.PlaceItem(0, 11);
            game.Map.ContainerContent().Returns(keys);
            threeSlotInventory.Open();
            outputDevice.Received(1)
                .ShowMessage(ThreeSlotInventory.OpenedContainerActionText);
            inputDevice.Received(1)
                .ReadCommand();
        }

        [TestMethod]
        public void LeavesItemInOpenedContainerIfPlayerDoesNotSwapOrPickUp()
        {
            game.Map.ItemInCurrentLocation().Returns(0);
            threeSlotInventory.Content[0] = 10;
            game.Map.Containers().Returns(new int?[] { 0, 1, 2, 3, 4, 5, 6 });
            var keys = new ContainerContent();
            keys.PlaceItem(0, 11);
            game.Map.ContainerContent().Returns(keys);
            inputDevice.ReadCommand().Returns(Edvella.Devices.CommandPalette.InvalidCommand);

            threeSlotInventory.Open();            
            outputDevice.DidNotReceive()
                .ShowMessage(ThreeSlotInventory.InventoryFullForContainerItemText);
        }

        [TestMethod]
        public void ShowsInventoryContentWhenSwappingOrPickingOpenedContainerItem()
        {
            game.Map.ItemInCurrentLocation().Returns(0);
            threeSlotInventory.Content[0] = 10;
            game.Map.Containers().Returns(new int?[] { 0, 1, 2, 3, 4, 5, 6 });
            var keys = new ContainerContent();
            keys.PlaceItem(0, 11);
            game.Map.ContainerContent().Returns(keys);
            inputDevice.ReadCommand().Returns(CommandPalette.Swap);
            inputDevice.ChooseListItem(Arg.Any<string>()).Returns(1);

            threeSlotInventory.Open();
            outputDevice.Received(1).ShowMessage("You hold");
            outputDevice.Received(1).ShowMessage("1. a silver key");
        }

        [TestMethod]
        public void CannotPickItemFromOpenedContainerIfInventoryIsFull()
        {
            game.Map.ItemInCurrentLocation().Returns(0);
            threeSlotInventory.Content[0] = 10;
            threeSlotInventory.Content[1] = 20;
            threeSlotInventory.Content[2] = 26;
            game.Map.Containers().Returns(new int?[] { 0, 1, 2, 3, 4, 5, 6 });
            var keys = new ContainerContent();
            keys.PlaceItem(0, 11);
            game.Map.ContainerContent().Returns(keys);
            inputDevice.ReadCommand().Returns(CommandPalette.PickUp, CommandPalette.InvalidCommand);

            threeSlotInventory.Open();
            outputDevice.Received(1)
                .ShowMessage(ThreeSlotInventory.InventoryFullForContainerItemText);
        }

        [TestMethod]
        public void PromptsPlayerToChooseItemToSwapWithOpenedContainerContent()
        {
            game.Map.ItemInCurrentLocation().Returns(0);
            threeSlotInventory.Content[0] = 10;
            threeSlotInventory.Content[1] = 20;
            threeSlotInventory.Content[2] = 26;
            game.Map.Containers().Returns(new int?[] { 0, 1, 2, 3, 4, 5, 6 });
            var keys = new ContainerContent();
            keys.PlaceItem(0, 11);
            game.Map.ContainerContent().Returns(keys);
            inputDevice.ReadCommand().Returns(CommandPalette.Swap);
            inputDevice.ChooseListItem(Arg.Any<string>()).Returns(1);

            threeSlotInventory.Open();
            inputDevice.Received(1)
                .ChooseListItem(ThreeSlotInventory.InventorySwapForOpenedContainerPrompt);
        }

        [TestMethod]
        public void CanOnlyChooseOneOfThreeSlotsForSwappingWithOpenedContanerContents()
        {
            game.Map.ItemInCurrentLocation().Returns(0);
            threeSlotInventory.Content[0] = 10;
            threeSlotInventory.Content[1] = 20;
            threeSlotInventory.Content[2] = 26;
            game.Map.Containers().Returns(new int?[] { 0, 1, 2, 3, 4, 5, 6 });
            var keys = new ContainerContent();
            keys.PlaceItem(0, 11);
            game.Map.ContainerContent().Returns(keys);
            inputDevice.ReadCommand().Returns(CommandPalette.Swap);
            inputDevice.ChooseListItem(Arg.Any<string>()).Returns(0, 4, 1);

            threeSlotInventory.Open();
            inputDevice.Received(3)
                .ChooseListItem(ThreeSlotInventory.InventorySwapForOpenedContainerPrompt);
        }

        [TestMethod]
        public void CannotSwapOpenedContanerContentsWithAnEmptyInventorySlot()
        {
            game.Map.ItemInCurrentLocation().Returns(0);
            threeSlotInventory.Content[0] = 10;
            threeSlotInventory.Content[2] = 26;
            game.Map.Containers().Returns(new int?[] { 0, 1, 2, 3, 4, 5, 6 });
            var keys = new ContainerContent();
            keys.PlaceItem(0, 11);
            game.Map.ContainerContent().Returns(keys);
            inputDevice.ReadCommand().Returns(CommandPalette.Swap);
            inputDevice.ChooseListItem(Arg.Any<string>()).Returns(2, 1);

            threeSlotInventory.Open();
            inputDevice.Received(2)
                .ChooseListItem(ThreeSlotInventory.InventorySwapForOpenedContainerPrompt);
        }

        [TestMethod]
        public void SwappingOpenedContainerItemPutsItemInInventory()
        {
            game.Map.ItemInCurrentLocation().Returns(0);
            threeSlotInventory.Content[0] = 10;
            threeSlotInventory.Content[2] = 26;
            game.Map.Containers().Returns(new int?[] { 0, 1, 2, 3, 4, 5, 6 });
            var keys = new ContainerContent();
            keys.PlaceItem(0, 11);
            game.Map.ContainerContent().Returns(keys);
            inputDevice.ReadCommand().Returns(CommandPalette.Swap);
            inputDevice.ChooseListItem(Arg.Any<string>()).Returns(1);

            threeSlotInventory.Open();
            threeSlotInventory.Content[0].Should().Be(11);
        }

        [TestMethod]
        public void SwappingOpenedContainerItemPutsInventoryItemInContainer()
        {
            game.Map.ItemInCurrentLocation().Returns(0);
            threeSlotInventory.Content[0] = 10;
            threeSlotInventory.Content[2] = 26;
            game.Map.Containers().Returns(new int?[] { 0, 1, 2, 3, 4, 5, 6 });
            var keys = new ContainerContent();
            keys.PlaceItem(0, 11);
            game.Map.ContainerContent().Returns(keys);
            inputDevice.ReadCommand().Returns(CommandPalette.Swap);
            inputDevice.ChooseListItem(Arg.Any<string>()).Returns(1);

            threeSlotInventory.Open();
            game.Map.ContainerContent().ItemLocations().ElementAt(0).Should().Be(10);
        }

        [TestMethod]
        public void PickingUpItemFromOpenedContainerAddsItToInventory()
        {
            game.Map.ItemInCurrentLocation().Returns(0);
            threeSlotInventory.Content[0] = 10;
            threeSlotInventory.Content[1] = 20;
            game.Map.Containers().Returns(new int?[] { 0, 1, 2, 3, 4, 5, 6 });
            var keys = new ContainerContent();
            keys.PlaceItem(0, 11);
            game.Map.ContainerContent().Returns(keys);
            inputDevice.ReadCommand().Returns(CommandPalette.PickUp, CommandPalette.InvalidCommand);

            threeSlotInventory.Open();
            threeSlotInventory.Content[2].Should().Be(11);
        }
    }
}
