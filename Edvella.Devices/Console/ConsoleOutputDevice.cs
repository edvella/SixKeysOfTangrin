using Edvella.Components;

namespace Edvella.Devices.Console
{
    public class ConsoleOutputDevice : IOutputDevice
    {
        private readonly IMessage message;
        private readonly ITitle title;

        public ConsoleOutputDevice()
        {
            message = new ConsoleMessage();
            title = new ConsoleTitle();
        }

        public void ShowMessage(string text)
        {
            System.Console.WriteLine(message.Render(text) as string);
        }

        public void ShowTitle(string title)
        {
            ShowMessage(this.title.Render(title) as string);
        }

        public void Clear()
        {
            System.Console.Clear();
        }
    }
}
