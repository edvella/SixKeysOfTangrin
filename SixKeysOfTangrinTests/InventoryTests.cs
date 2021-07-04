using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SixKeysOfTangrin;

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
                outputDevice, inputDevice, game.Map);

            game.Map.ItemDescription(11).Returns("a golden key");
            game.Map.ItemDescription(20).Returns("a French dictionary");
            game.Map.ItemDescription(16).Returns("a length of rope");
            game.Map.ItemDescription(28).Returns("a pile of radio components");
        }

        [TestMethod]
        public void ShowsMessageWhenThereIsNothingToPickUp()
        {
            game.Map.ItemInCurrentLocation().Returns(29);
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
            game.Map.ItemInCurrentLocation().Returns(28);
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
            game.Map.ItemInCurrentLocation().Returns(28);
            threeSlotInventory.PickUp();
            outputDevice.DidNotReceive().ShowMessage("You hold the following");
        }

        [TestMethod]
        public void PickupFailsIfInventoryIsFull()
        {
            game.Map.ItemInCurrentLocation().Returns(28);
            threeSlotInventory.Content[0] = 11;
            threeSlotInventory.Content[1] = 20;
            threeSlotInventory.Content[2] = 16;
            threeSlotInventory.PickUp();
            outputDevice.Received(1).ShowMessage(ThreeSlotInventory.InventoryFullText);
        }

        [TestMethod]
        public void ConfirmsWhenItemIsPickedUp()
        {
            game.Map.ItemInCurrentLocation().Returns(28);
            threeSlotInventory.PickUp();
            outputDevice.Received(1).ShowMessage("OK I've picked up the pile of radio components.");
        }

        [TestMethod]
        public void CanPickOneItemUpAndAddToInventory()
        {
            game.Map.ItemInCurrentLocation().Returns(28);
            threeSlotInventory.PickUp();
            threeSlotInventory.Content[0].Should().Be(28);
        }

        [TestMethod]
        public void CanPickTwoItemsUpAndAddToInventory()
        {
            game.Map.ItemInCurrentLocation().Returns(16);
            threeSlotInventory.PickUp();
            game.Map.ItemInCurrentLocation().Returns(28);
            threeSlotInventory.PickUp();

            threeSlotInventory.Content[0].Should().Be(16);
            threeSlotInventory.Content[1].Should().Be(28);
        }

        [TestMethod]
        public void CanPickThreeItemsUpAndAddToInventory()
        {
            game.Map.ItemInCurrentLocation().Returns(16);
            threeSlotInventory.PickUp();
            game.Map.ItemInCurrentLocation().Returns(11);
            threeSlotInventory.PickUp();
            game.Map.ItemInCurrentLocation().Returns(28);
            threeSlotInventory.PickUp();

            threeSlotInventory.Content[0].Should().Be(16);
            threeSlotInventory.Content[1].Should().Be(11);
            threeSlotInventory.Content[2].Should().Be(28);
        }

        [TestMethod]
        public void PickedUpItemsAreRemovedFromTheMap()
        {
            game.Map.ItemInCurrentLocation().Returns(28);
            threeSlotInventory.PickUp();
            game.Map.Received(1).RemoveItemFromCurrentLocation();
        }

        [TestMethod]
        public void CannotDumpItemIfThereIsAnItemAlreadyInTheCurrentLocation()
        {
            game.Map.ItemInCurrentLocation().Returns(28);
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
        public void PromptsItemChoiceAgainIfIncorrectChoiceNumberIsSelected()
        {
            game.Map.ItemInCurrentLocation().Returns(ItemCollection.Nothing);
            threeSlotInventory.Content[0] = 11;
            inputDevice.ChooseListItem(Arg.Any<string>()).Returns(0, 1);
            threeSlotInventory.Dump();
            inputDevice.Received(2).ChooseListItem(ThreeSlotInventory.InventoryChoiceText);
        }

        [TestMethod]
        public void PromptsItemChoiceAgainIfEmptySlotIsChosen()
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
    }
}
