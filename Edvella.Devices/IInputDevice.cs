namespace Edvella.Devices;

public interface IInputDevice
{
    YesNo YesNoPrompt(string text);
    string ContinueInstructions();
    void WaitForPlayerToContinue();
    CommandPalette ReadCommand();
    int ChooseListItem(string text);
}
