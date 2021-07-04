using Edvella.Components;

namespace Edvella.Devices.Console
{
    public class ConsoleTitle : ITitle
    {
        public object Render(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            return $"{text}\n{new string('*', text.Length)}\n\n";
        }
    }
}
