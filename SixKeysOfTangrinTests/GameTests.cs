using Edvella.Devices;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SixKeysOfTangrin;
using System.Threading.Tasks;

namespace SixKeysOfTangrinTests
{
    [TestClass]
    public class GameTests : TestsBase
    {
        private const string TideTimerMessage = "You have 200 minutes before the tide returns";

        [TestMethod]
        public void CanShowGameTitle()
        {
            game.ShowTitle();
            outputDevice.Received().ShowTitle(Game.Title);
        }

        [TestMethod]
        public void TideTimerStartsFrom200()
        {
            game.TideTime.Should().Be(Game.TideOutDuration);
        }

        [TestMethod]
        public void DisplaysRemainingTimeBeforeTideReturnsIfNotZero()
        {
            game.NextTurn();
            outputDevice.Received().ShowMessage(TideTimerMessage);
        }

        [TestMethod]
        public void DoesNotDisplayRemainingTideTimeIfExpired()
        {
            outputDevice.DidNotReceive().ShowMessage(TideTimerMessage);
        }

        [TestMethod]
        public void TideTimerIsReducedEveryTurn()
        {
            game.NextTurn();
            game.TideTime.Should().BeInRange(183, 200);
        }

        [TestMethod]
        public void DisplaysMessageWhenTideIsIn()
        {
            ExpireTideTimer();
            game.ShowIfTideIn();
            outputDevice.Received().ShowMessage(Game.TideIsInText);
        }

        [TestMethod]
        public void DoesNotDisplayAnyMessageIfTideIsNotInYet()
        {
            game.ShowIfTideIn();
            outputDevice.DidNotReceive().ShowMessage(Game.TideIsInText);
        }

        [TestMethod]
        public void PlayerCarriedAwayIfTideIsInAndUnderCliffNearHouse()
        {
            game.Map.PlayerLocation = TangrinMap.StartingLocation;
            ExpireTideTimer();
            game.ShowIfTideIn();
            outputDevice.Received().ShowMessage(Game.PlayerCarriedAwayText);
        }

        [TestMethod]
        public void PlayerEndsUpInAnotherLocationIfTideIsInAndUnderCliffNearHouse()
        {
            game.Map.PlayerLocation = TangrinMap.StartingLocation;
            ExpireTideTimer();
            game.ShowIfTideIn();
            game.Map.PlayerLocation.Should().NotBe(TangrinMap.StartingLocation);
        }

        [TestMethod]
        public void PlayerCannotBeCarriedAwayOutsideOfMap()
        {
            game.Map.PlayerLocation = TangrinMap.StartingLocation;
            ExpireTideTimer();
            game.ShowIfTideIn();
            game.Map.PlayerLocation.Should().BeInRange(1, 29);
        }

        [TestMethod]
        public void PlayerIsNotCarriedAwayWhenNotUnderCliffNearHouse()
        {
            game.Map.PlayerLocation = 2;
            ExpireTideTimer();
            game.ShowIfTideIn();
            outputDevice.DidNotReceive().ShowMessage(Game.PlayerCarriedAwayText);
        }

        [TestMethod]
        public void PlayerIsNotCarriedAwayWhenTideIsOut()
        {
            game.Map.PlayerLocation = TangrinMap.StartingLocation;
            game.ShowIfTideIn();
            outputDevice.DidNotReceive().ShowMessage(Game.PlayerCarriedAwayText);
        }

        [TestMethod]
        public void PlayerStaysInSameLocationWhenNotUnderCliffNearHouseAndTideIsIn()
        {
            game.Map.PlayerLocation = 2;
            ExpireTideTimer();
            game.ShowIfTideIn();
            game.Map.PlayerLocation.Should().Be(2);
        }

        [TestMethod]
        public void PlayerStaysInSameLocationWhenUnderCliffNearHouseButTideIsOut()
        {
            game.Map.PlayerLocation = TangrinMap.StartingLocation;
            game.ShowIfTideIn();
            game.Map.PlayerLocation.Should().Be(TangrinMap.StartingLocation);
        }

        [TestMethod]
        public void PlayerEnergyIsReducedEachTurnIfNotUnderCliffNearHouse()
        {
            game.Map.PlayerLocation = 2;
            game.NextTurn();
            player.Received(1).Drain(Arg.Any<int>());
            player.DidNotReceive().RestoreFullHealth();
        }

        [TestMethod]
        public void PlayerEnergyIsFullyRechargedIfUnderCliffNearHouseAndTideIsOut()
        {
            game.Map.PlayerLocation = 2;
            game.NextTurn();
            game.Map.PlayerLocation = TangrinMap.StartingLocation;
            game.NextTurn();
            player.Received(1).RestoreFullHealth();
        }

        [TestMethod]
        public void PlayerEnergyIsReducedEachTurnWhenTideIsInEvenIfUnderCliffNearHouse()
        {
            game.Map.PlayerLocation = TangrinMap.StartingLocation;
            ExpireTideTimer();
            game.NextTurn();
            player.Received(1).Drain(Arg.Any<int>());
            player.DidNotReceive().RestoreFullHealth();
        }

        [TestMethod]
        public void TideStaysInFor100MinutesUntilItResets()
        {
            ExpireTideTimer();

            while (game.TideTime != Game.TideOutDuration)
                game.NextTurn();

            game.TideTime.Should().Be(Game.TideOutDuration);
        }

        [TestMethod]
        public void EachTurnDisplaysTheLocationDescription()
        {
            game.Map.PlayerLocation = 10;
            game.NextTurn();
            outputDevice.Received(1).ShowMessage(TangrinMap.caveDescriptions[10]);
        }

        [TestMethod]
        public void EachTurnDisplaysTheItemDescription()
        {
            UseGameWithMockedMap();
            game.NextTurn();
            game.Map.Received(1).VisibleItem();
        }

        private void ExpireTideTimer()
        {
            game.TideTime = -1;
        }

        [TestMethod]
        public void CanRandomlyMeetTheGhostOfTangrin()
        {
            randomGenerator = Substitute.For<IRandomGenerator>();
            randomGenerator.NextDouble().Returns(.9858, .9859);
            game = new Game(
                inputDevice,
                outputDevice,
                randomGenerator,
                MapInstance(),
                suspense,
                player,
                inventory);

            game.Map.Initialise();

            game.NextTurn();
            outputDevice.DidNotReceive().ShowMessage(Game.MeetTangrinText);
            game.NextTurn();
            outputDevice.Received(1).ShowMessage(Game.MeetTangrinText);
        }

        [TestMethod]
        public void PossibleExitsAreShownEachTurn()
        {
            UseGameWithMockedMap();
            game.NextTurn();
            game.Map.Received(1).CurrentExits();
        }

        [TestMethod]
        public void ShowsAvailableCommands()
        {
            UseGameWithMockedMap();
            game.NextTurn();
            outputDevice.Received(1).ShowMessage(Game.CommandsText);
        }

        [TestMethod]
        public void EachTurnShowEnergyStatus()
        {
            game.NextTurn();
            game.Player.Received(1).EnergyStatus();
        }

        [TestMethod]
        public void EachTurnGetsPlayerCommand()
        {
            game.NextTurn();
            inputDevice.Received(1).ReadCommand();
        }

        [TestMethod]
        public void WinsGameWhenHoldingTreasureAtEntrance()
        {
            SetUpWinningCondition();

            game.NextTurn();
            outputDevice.Received(1).ShowMessage(TangrinMap.GotOutWithTreasureText);
        }

        private void SetUpWinningCondition()
        {
            UseGameWithMockedMap();
            game.Map.GoNorth().Returns(true);
            game.Map.PlayerLocation.Returns(TangrinMap.StartingLocation);
            inventory.IsHolding(ItemCollection.Treasure).Returns(true);
            inputDevice.ReadCommand().Returns(CommandPalette.North);
        }

        [TestMethod]
        public void DoesNotWinGameIfNotHoldingTreasure()
        {
            SetUpWinningCondition();
            inventory.IsHolding(ItemCollection.Treasure).Returns(false);

            game.NextTurn();
            outputDevice.DidNotReceive().ShowMessage(TangrinMap.GotOutWithTreasureText);
        }

        [TestMethod]
        public void DoesNotWinGameIfPlayerIsNotInStartingLocation()
        {
            SetUpWinningCondition();
            game.Map.PlayerLocation.Returns(5);

            game.NextTurn();
            outputDevice.DidNotReceive().ShowMessage(TangrinMap.GotOutWithTreasureText);
        }

        [TestMethod]
        public void DoesNotWinGameIfTideIsIn()
        {
            SetUpWinningCondition();
            game.TideTime = -1;

            game.NextTurn();
            outputDevice.DidNotReceive().ShowMessage(TangrinMap.GotOutWithTreasureText);
        }

        [TestMethod]
        public void PromptsPlayerForAnotherGoAfterWinning()
        {
            SetUpWinningCondition();
            
            game.NextTurn();
            outputDevice.Received(1).ShowMessage(Game.PlayAgainText);
            inputDevice.Received(1).WaitForPlayerToContinue();
        }

        [TestMethod]
        public void EndsGameWhenPlayerDies()
        {
            game.Player.IsDead().Returns(true);
            game.NextTurn();
            game.IsEndTriggered.Should().BeTrue();
        }
    }
}
