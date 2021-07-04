using Edvella.Components;

namespace Edvella.Devices.Console
{
    public class ConsoleMessage : IMessage
    {
        public object Render(string text)
        {
            return text;
        }
    }
}
