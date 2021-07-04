using Edvella.Devices;
using System.Linq;

namespace SixKeysOfTangrin
{
    public class ThreeSlotInventory : IInventory
    {
        public const string NothingToPickupText = "Nothing here";
        public const string MagicalForceText = "A magical force stops you from taking it";
        public const string YouHoldText = "You hold the following";
        public const string InventoryFullText = "Sorry no hands or pockets";
        public const string PickupConfirmedText = "OK I've picked up the";
        public const string ItemAlredyPresentText = "Sorry there is already something here";
        public const string InventoryEmptyText = "You don't have anything with you!";
        public const string InventoryChoiceText = "Which one (1-3)";

        private readonly IOutputDevice outputdevice;
        private readonly IInputDevice inputDevice;
        private readonly IMap map;

        public ThreeSlotInventory(
            IOutputDevice outputdevice, IInputDevice inputDevice, IMap map)
        {
            this.outputdevice = outputdevice;
            this.inputDevice = inputDevice;
            this.map = map;
        }

        public int?[] Content { get; set; } = new int?[3];

        public bool PickUp()
        {
            var item = map.ItemInCurrentLocation();
            if (item == ItemCollection.Nothing)
                outputdevice.ShowMessage(NothingToPickupText);
            else if (item < 7)
                outputdevice.ShowMessage(MagicalForceText);
            else
            {
                List();
                if (Full())
                    outputdevice.ShowMessage(InventoryFullText);
                else
                    Insert(item);
            }

            return false;
        }

        public bool Dump()
        {
            if (map.ItemInCurrentLocation() != ItemCollection.Nothing)
                outputdevice.ShowMessage(ItemAlredyPresentText);
            else if (Empty())
                outputdevice.ShowMessage(InventoryEmptyText);
            else
            {
                List();
                Remove();
            }

            return false;
        }

        private void Remove()
        {
            bool itemDumped = false;
            while (!itemDumped)
            {
                var itemToDump = inputDevice.ChooseListItem(InventoryChoiceText);
                if (itemToDump >= 1 && itemToDump <= 3 && Content[itemToDump - 1].HasValue)
                {
                    map.AddItemToCurrentLocation(Content[itemToDump - 1].Value);
                    Content[itemToDump - 1] = null;
                    itemDumped = true;
                }
            }
        }

        private void Insert(int item)
        {
            Content[FreeSlot().Value] = item;
            map.RemoveItemFromCurrentLocation();
            outputdevice.ShowMessage(
                $"{PickupConfirmedText} {map.ItemDescription(item)[2..]}.");
        }

        private int? FreeSlot()
        {
            for (var i = 0; i < Content.Length; i++)
                if (!Content[i].HasValue) return i;

            return null;
        }

        private void List()
        {
            if (Empty()) return;

            outputdevice.ShowMessage(YouHoldText);
            for (var i = 0; i < Content.Length; i++)
            {
                if (Content[i] != null)
                {
                    outputdevice.ShowMessage($"{i + 1}. {map.ItemDescription(Content[i].Value)}");
                }
            }
        }

        private bool Full()
        {
            return Content.All(_ => _.HasValue);
        }

        private bool Empty()
        {
            return Content.All(_ => !_.HasValue);
        }
    }
}
