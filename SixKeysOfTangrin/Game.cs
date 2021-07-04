using Edvella.Devices;
using SixKeysOfTangrin.Effects;
using System;
using System.Threading.Tasks;

namespace SixKeysOfTangrin
{
    public class Game
    {
        public const string Title = "The 6 Keys of Tangrin";
        public const string OfferInstructionsText = "Do you need instructions?";
        public const string InstructionsNotNeededText = "Oh all right - I'll just get on with it.";
        public const string InstructionsPart1 =
            @"You are on holiday in Cornwall and are staying in your aunt's 
old house near the sea.

The house lies in an area known as Smuggler's Den and the
townfolk will tell stories of clever smugglers hiding their
wares in the maze of caves around the coastline.";
        public const string TideIsInText = "Tide is in";
        public const string PlayerCarriedAwayText = 
            @"Sploosh!!!
Tide's in! - You are carried
to some deep cave.";

        public const int TideOutDuration = 200;

        public int TideTime { get; set; } = TideOutDuration;

        private readonly IRandomGenerator rnd;

        public const string InstructionsPart2 =
            @"It is said that one madman who lived in the house in the last
century had stored his treasure somewhere and locked it using
keys which were themselves locked in boxes!

However, many attempts have been made to recover the treasure
and so there may be keys left by previous frustrated explorers
- or so it is said: it is probably mostly gossip!";

        public const string InstructionsPart3 =
            @"You decide to do some exploring on your own. You go down
the sea front and walk to the cliff under the house.
The tide is out, but note to return within a few hours.
You are able to hold 3 things in your hands and pockets.
Should the need arise that is!";

        public const string MeetTangrinText =
            @"You meet up with the ghost of Tangrin who can take you out,
but he only speaks French";

        public const string CommandsText =
            "Type in: open, pickup, dump, swap, end, north, south, east or west.";

        public const string InvalidCommandText =
            "Don't understand your banter, squadron leader!";

        private readonly IInputDevice inputDevice;

        private readonly IOutputDevice outputDevice;

        private readonly ISuspense suspense;

        public IMap Map { get; private set; }
        public IPlayer Player { get; private set; }

        private readonly IInventory inventory;

        public bool IsEndTriggered { get; private set; }
        public bool IsTurnOver { get; set; }

        public Game(
            IInputDevice inputDevice, 
            IOutputDevice outputDevice,
            IRandomGenerator randomGenerator,
            IMap map,
            ISuspense suspense,
            IPlayer player,
            IInventory inventory)
        {
            this.inputDevice = inputDevice;
            this.outputDevice = outputDevice;
            this.suspense = suspense;
            rnd = randomGenerator;
            Map = map;
            Player = player;
            this.inventory = inventory;
        }

        public void Start()
        {
            outputDevice.Clear();
            ShowTitle();
            OfferInstructions();
            Map.Initialise();

            outputDevice.Clear();

            while(!IsEndTriggered)
                NextTurn();
        }

        public void ShowTitle()
        {
            outputDevice.ShowTitle(Title);
        }

        public void OfferInstructions()
        {
            var instructions = inputDevice.YesNoPrompt(OfferInstructionsText);
            if (instructions == YesNo.Yes)
                NeedInstructions();
            else
                InstructionsNotNeeded();
        }

        public void NeedInstructions()
        {
            AdvanceStory(InstructionsPart1);
            AdvanceStory(InstructionsPart2);
            AdvanceStory(InstructionsPart3);
        }

        public void AdvanceStory(string text)
        {
            outputDevice.Clear();
            outputDevice.ShowMessage(text);
            WaitToAdvanceStory();
        }

        public void WaitToAdvanceStory()
        {
            outputDevice.ShowMessage(inputDevice.ContinueInstructions());
            inputDevice.WaitForPlayerToContinue();
        }

        public void InstructionsNotNeeded()
        {
            outputDevice.ShowMessage(InstructionsNotNeededText);
        }

        public void NextTurn()
        {
            IsTurnOver = false;
            outputDevice.Clear();
            TideCheck();
            TideTime -= rnd.Next(18);
            ShowIfTideIn();
            CalculateEnergyLeft();
            if (TideTime < -100) TideTime = TideOutDuration;
            outputDevice.ShowMessage(Map.LookCommand());
            outputDevice.ShowMessage(Map.VisibleItem());
            suspense.Delay(1000);

            TangrinScene();
            PlayerMove();
        }

        private void TangrinScene()
        {
            if (rnd.NextDouble() > .9858)
            {
                outputDevice.ShowMessage(MeetTangrinText);
                suspense.Delay(4000);
                outputDevice.Clear();
            }
        }

        private void CalculateEnergyLeft()
        {
            if (Map.PlayerLocation == TangrinMap.StartingLocation && IsTideOut())
                Player.RestoreHealth();
            else
                Player.Drain(TideTime);
        }

        public void TideCheck()
        {
            if (IsTideOut())
                outputDevice.ShowMessage(
                    $"You have {TideTime} minutes before the tide returns");
        }

        public void ShowIfTideIn()
        {
            if (IsTideOut()) return;

            outputDevice.ShowMessage(TideIsInText);

            if (Map.PlayerLocation == TangrinMap.StartingLocation)
            {
                outputDevice.ShowMessage(PlayerCarriedAwayText);
                Map.PlayerLocation = rnd.Next(20) + 1;
            }
        }

        private bool IsTideOut()
        {
            return TideTime > 0;
        }

        private void PlayerMove()
        {
            while (!IsTurnOver)
            {
                outputDevice.ShowMessage(Map.CurrentExits());
                outputDevice.ShowMessage(CommandsText);
                outputDevice.ShowMessage(Player.EnergyStatus());

                var choice = inputDevice.ReadCommand();
                switch (choice)
                {
                    case CommandPalette.North:
                        IsTurnOver = Map.GoNorth();
                        break;
                    case CommandPalette.East:
                        IsTurnOver = Map.GoEast();
                        break;
                    case CommandPalette.Up:
                        IsTurnOver = Map.GoUp();
                        break;
                    case CommandPalette.South:
                        IsTurnOver = Map.GoSouth();
                        break;
                    case CommandPalette.West:
                        IsTurnOver = Map.GoWest();
                        break;
                    case CommandPalette.Down:
                        IsTurnOver = Map.GoDown();
                        break;
                    case CommandPalette.End:
                        IsEndTriggered = true;
                        IsTurnOver = true;
                        break;
                    case CommandPalette.PickUp:
                        IsTurnOver = inventory.PickUp();
                        break;
                    case CommandPalette.Dump:
                        IsTurnOver = inventory.Dump();
                        break;
                    default:
                        outputDevice.ShowMessage(InvalidCommandText);
                        break;
                }
            }
        }
    }
}
