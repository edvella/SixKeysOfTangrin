namespace SixKeysOfTangrinTests;

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

    [DataRow(9)]
    [DataRow(2)]
    [DataTestMethod]
    public void WarnsWhenPlayerIsExhausted(int amount)
    {
        player.Energy = amount;
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

    [DataRow(10)]
    [DataRow(19)]
    [DataTestMethod]
    public void WarnsWhenPlayerIsWeary(int amount)
    {
        player.Energy = amount;
        player.EnergyStatus().Should().Be(Player.Weary);
    }

    [DataRow(20)]
    [DataRow(29)]
    [DataTestMethod]
    public void WarnsWhenPlayerIsTired(int amount)
    {
        player.Energy = amount;
        player.EnergyStatus().Should().Be(Player.Tired);
    }

    [DataRow(30)]
    [DataRow(39)]
    [DataTestMethod]
    public void ShowsWhenPlayerIsStillFit(int amount)
    {
        player.Energy = amount;
        player.EnergyStatus().Should().Be(Player.Peckish);
    }

    [DataRow(40)]
    [DataRow(49)]
    [DataTestMethod]
    public void DoesNotShowAnyStatusWhenPlayerIsFitButNotSoStrong(int amount)
    {
        player.Energy = amount;
        player.EnergyStatus().Should().BeNull();
    }

    [DataRow(50)]
    [DataRow(59)]
    [DataTestMethod]
    public void WarnsWhenPlayerBecomesLessStrong(int amount)
    {
        player.Energy = amount;
        player.EnergyStatus().Should().Be(Player.LessStrong);
    }

    [DataRow(60)]
    [DataRow(79)]
    [DataTestMethod]
    public void ShowsWhenPlayerIsStillStrongEnough(int amount)
    {
        player.Energy = amount;
        player.EnergyStatus().Should().Be(Player.Strong);
    }

    [DataRow(80)]
    [DataRow(89)]
    [DataTestMethod]
    public void ShowsWhenPlayerIsAt100PercentEfficiency(int amount)
    {
        player.Energy = amount;
        player.EnergyStatus().Should().Be(Player.Efficient);
    }

    [DataRow(90)]
    [DataRow(99)]
    [DataTestMethod]
    public void ShowsWhenPlayerIsAtFullStrength(int amount)
    {
        player.Energy = amount;
        player.EnergyStatus().Should().Be(Player.FullStrength);
    }

    [TestMethod]
    public void PlayerEnergyCanBePartiallyRestored()
    {
        player.Restore(60);
        player.Energy.Should().Be(159);
    }

    [TestMethod]
    public void PlayerEnergyCanBeFullyRestored()
    {
        player.Energy = 1;
        player.RestoreFullHealth();
        player.Energy.Should().Be(Player.FullEnergy);
    }
}
