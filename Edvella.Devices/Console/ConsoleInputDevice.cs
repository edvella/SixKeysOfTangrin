using System;

namespace Edvella.Devices.Console
{
    public class ConsoleInputDevice : IInputDevice
    {
        public YesNo YesNoPrompt(string text)
        {
            System.Console.Write(text);
            var userReply = System.Console.ReadKey();

            if (userReply.Key == ConsoleKey.Y)
                return YesNo.Yes;
            else
                return YesNo.No;
        }

        public string ContinueInstructions()
        {
            return "please hit ENTER";
        }

        public void WaitForPlayerToContinue()
        {
            var userReply = System.Console.ReadKey();

            if (userReply.Key != ConsoleKey.Enter)
                WaitForPlayerToContinue();
        }

        public CommandPalette ReadCommand()
        {
            return UserInput() switch
            {
                "OP" => CommandPalette.Open,
                "PI" => CommandPalette.PickUp,
                "DU" => CommandPalette.Dump,
                "SW" => CommandPalette.Swap,
                "EN" => CommandPalette.End,
                "NO" => CommandPalette.North,
                "SO" => CommandPalette.South,
                "EA" => CommandPalette.East,
                "WE" => CommandPalette.West,
                "UP" => CommandPalette.Up,
                "DO" => CommandPalette.Down,
                _ => CommandPalette.InvalidCommand,
            };
        }

        private static string UserInput()
        {
            var command = System.Console.ReadLine();
            if (command.Length >= 2)
                return command.Substring(0, 2).ToUpperInvariant();

            return command;
        }

        public int ChooseListItem(string text)
        {
            System.Console.Write(text);
            int.TryParse(System.Console.ReadLine(), out int choice);
            return choice;
        }
    }
}
