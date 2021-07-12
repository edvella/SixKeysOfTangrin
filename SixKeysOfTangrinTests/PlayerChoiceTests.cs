using Edvella.Devices;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SixKeysOfTangrin;

namespace SixKeysOfTangrinTests
{
    [TestClass]
    public class PlayerChoiceTests : TestsBase
    {
        [TestInitialize]
        public void Init()
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

        [TestMethod]
        public void PromptsPlayerIfInstructionsNeeded()
        {
            game.OfferInstructions();
            inputDevice.Received().YesNoPrompt(Game.OfferInstructionsText);
        }

        [TestMethod]
        public void ShowsInstructionsIfPlayerAccepts()
        {
            inputDevice
                .YesNoPrompt(Game.OfferInstructionsText)
                .Returns(YesNo.Yes);

            game.OfferInstructions();
            outputDevice.Received().ShowMessage(Game.InstructionsPart1);
            outputDevice.Received().ShowMessage(Game.InstructionsPart2);
            outputDevice.Received().ShowMessage(Game.InstructionsPart3);
        }

        [TestMethod]
        public void IfPlayerDoesNotWantInstructionsShowMessageToProceed()
        {
            inputDevice
                .YesNoPrompt(Game.OfferInstructionsText)
                .Returns(YesNo.No);

            game.OfferInstructions();
            outputDevice.Received().ShowMessage(Game.InstructionsNotNeededText);
        }

        [TestMethod]
        public void NorthCommandTakesPlayerNorth()
        {
            inputDevice.ReadCommand().Returns(CommandPalette.North);
            game.Map.GoNorth().Returns(true);
            game.NextTurn();
            game.Map.Received(1).GoNorth();
        }

        [TestMethod]
        public void EastCommandTakesPlayerEast()
        {
            inputDevice.ReadCommand().Returns(CommandPalette.East);
            game.Map.GoEast().Returns(true);
            game.NextTurn();
            game.Map.Received(1).GoEast();
        }

        [TestMethod]
        public void UpCommandTakesPlayerUp()
        {
            inputDevice.ReadCommand().Returns(CommandPalette.Up);
            game.Map.GoUp().Returns(true);
            game.NextTurn();
            game.Map.Received(1).GoUp();
        }

        [TestMethod]
        public void SouthCommandTakesPlayerSouth()
        {
            inputDevice.ReadCommand().Returns(CommandPalette.South);
            game.Map.GoSouth().Returns(true);
            game.NextTurn();
            game.Map.Received(1).GoSouth();
        }

        [TestMethod]
        public void WestCommandTakesPlayerWest()
        {
            inputDevice.ReadCommand().Returns(CommandPalette.West);
            game.Map.GoWest().Returns(true);
            game.NextTurn();
            game.Map.Received(1).GoWest();
        }

        [TestMethod]
        public void DownCommandTakesPlayerDown()
        {
            inputDevice.ReadCommand().Returns(CommandPalette.Down);
            game.Map.GoDown().Returns(true);
            game.NextTurn();
            game.Map.Received(1).GoDown();
        }

        [TestMethod]
        public void ShowMessageIfPlayerCommandNotRecognised()
        {
            inputDevice.ReadCommand().Returns(
                CommandPalette.InvalidCommand,
                CommandPalette.End);
            game.NextTurn();
            outputDevice.Received(1).ShowMessage(Game.InvalidCommandText);
        }

        [TestMethod]
        public void EndCommandExitsGame()
        {
            inputDevice.ReadCommand().Returns(CommandPalette.End);
            game.NextTurn();
            game.IsEndTriggered.Should().BeTrue();
        }

        [TestMethod]
        public void PlayerCanPickItemsUp()
        {
            inputDevice.ReadCommand().Returns(CommandPalette.PickUp);
            inventory.PickUp().Returns(true);
            game.NextTurn();
            inventory.Received(1).PickUp();
        }

        [TestMethod]
        public void PlayerCanDumpItems()
        {
            inputDevice.ReadCommand().Returns(CommandPalette.Dump);
            inventory.Dump().Returns(true);
            game.NextTurn();
            inventory.Received(1).Dump();
        }

        [TestMethod]
        public void PlayerCanSwapItems()
        {
            inputDevice.ReadCommand().Returns(CommandPalette.Swap);
            inventory.Swap().Returns(true);
            game.NextTurn();
            inventory.Received(1).Swap();
        }

        [TestMethod]
        public void PlayerCanOpenStuff()
        {
            inputDevice.ReadCommand().Returns(CommandPalette.Open);
            inventory.Open().Returns(true);
            game.NextTurn();
            inventory.Received(1).Open();
        }
    }
}
