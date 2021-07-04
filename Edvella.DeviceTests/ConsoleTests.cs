using Edvella.Devices.Console;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Edvella.DeviceTests
{
    [TestClass]
    public class ConsoleTests
    {
        private readonly ConsoleTitle title;
        public ConsoleTests()
        {
            title = new ConsoleTitle();
        }

        [TestMethod]
        public void ShortTitleIsUnderlinedWithAsterisks()
        {
            ((string)title.Render("X")).Should().StartWith("X\n*");
        }

        [TestMethod]
        public void StandardTitleIsUnderlinedWithAsterisks()
        {
            ((string)title.Render("Pretty Standard Title"))
                .Should().StartWith("Pretty Standard Title\n*********************");
        }

        [TestMethod]
        public void TitleIsUnderlinedWithAsterisks()
        {
            title.Render(string.Empty)
                .Should().Be(string.Empty);
        }

        [TestMethod]
        public void TitleIsFollowedByTwoNewLines()
        {
            ((string)title.Render("Anything"))
                .Should().EndWith("\n\n");
        }
    }
}
