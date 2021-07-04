using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SixKeysOfTangrin;

namespace SixKeysOfTangrinTests
{
    [TestClass]
    public class PlayerTests
    {
        private Player player;

        [TestInitialize]
        public void Init()
        {
            player = new Player();
        }

        [TestMethod]
        public void PlayerStartsGameWithFullEnergy()
        {
            player.Energy.Should().Be(Player.FullEnergy);
        }

        [TestMethod]
        public void PlayerEnergyCanBeDrained()
        {
            player.Drain(200);
            player.Energy.Should().BeLessThan(Player.FullEnergy);
        }

        [TestMethod]
        public void WarnsWhenPlayerIsExhausted()
        {
            player.Energy = 9;
            player.EnergyStatus().Should().Be(Player.Exhausted);
            player.EnergyStatus().Should().NotContain(Player.Died);
            player.Energy = 2;
            player.EnergyStatus().Should().Be(Player.Exhausted);
            player.EnergyStatus().Should().NotContain(Player.Died);
        }

        [TestMethod]
        public void ShowsWhenPlayerDiesOfExhaustion()
        {
            player.Energy = 1;
            player.EnergyStatus().Should().Contain(Player.Exhausted);
            player.EnergyStatus().Should().Contain(Player.Died);
        }

        [TestMethod]
        public void WarnsWhenPlayerIsWeary()
        {
            player.Energy = 10;
            player.EnergyStatus().Should().Be(Player.Weary);
            player.Energy = 19;
            player.EnergyStatus().Should().Be(Player.Weary);
        }

        [TestMethod]
        public void WarnsWhenPlayerIsTired()
        {
            player.Energy = 20;
            player.EnergyStatus().Should().Be(Player.Tired);
            player.Energy = 29;
            player.EnergyStatus().Should().Be(Player.Tired);
        }

        [TestMethod]
        public void ShowsWhenPlayerIsStillFit()
        {
            player.Energy = 30;
            player.EnergyStatus().Should().Be(Player.Peckish);
            player.Energy = 39;
            player.EnergyStatus().Should().Be(Player.Peckish);
        }

        [TestMethod]
        public void DoesNotShowAnyStatusWhenPlayerIsFitButNotSoStrong()
        {
            player.Energy = 40;
            player.EnergyStatus().Should().BeNull();
            player.Energy = 49;
            player.EnergyStatus().Should().BeNull();
        }

        [TestMethod]
        public void WarnsWhenPlayerBecomesLessStrong()
        {
            player.Energy = 50;
            player.EnergyStatus().Should().Be(Player.LessStrong);
            player.Energy = 59;
            player.EnergyStatus().Should().Be(Player.LessStrong);
        }

        [TestMethod]
        public void ShowsWhenPlayerIsStillStrongEnough()
        {
            player.Energy = 60;
            player.EnergyStatus().Should().Be(Player.Strong);
            player.Energy = 79;
            player.EnergyStatus().Should().Be(Player.Strong);
        }

        [TestMethod]
        public void ShowsWhenPlayerIsAt100PercentEfficiency()
        {
            player.Energy = 80;
            player.EnergyStatus().Should().Be(Player.Efficient);
            player.Energy = 89;
            player.EnergyStatus().Should().Be(Player.Efficient);
        }

        [TestMethod]
        public void ShowsWhenPlayerIsAtFullStrength()
        {
            player.Energy = 90;
            player.EnergyStatus().Should().Be(Player.FullStrength);
            player.Energy = 99;
            player.EnergyStatus().Should().Be(Player.FullStrength);
        }
    }
}
