namespace Edvella.Devices
{
    public interface IOutputDevice
    {
        void ShowMessage(string text);
        void ShowTitle(string title);
        void Clear();
    }
}