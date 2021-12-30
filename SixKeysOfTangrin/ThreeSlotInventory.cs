namespace SixKeysOfTangrin;

public class ThreeSlotInventory : IInventory
{
    public const string NothingToPickupText = "Nothing here.";
    public const string MagicalForceText = "A magical force stops you from taking it.";
    public const string YouHoldText = "You hold the following";
    public const string InventoryFullText = "Sorry no hands or pockets.";
    public const string InventoryFullForContainerItemText = "You hold 3 items.";
    public const string PickupConfirmedText = "OK I've picked up the";
    public const string ItemAlredyPresentText = "Sorry there is already something here.";
    public const string InventoryEmptyText = "You don't have anything with you!";
    public const string InventoryChoiceText = "Which one (1-3)";
    public const string ObjectTooHeavyText = "Sorry- too heavy to move.";
    public const string NothingToSwapText = "You have nothing to swap!";
    public const string SwapItemSelectionText = "PLEASE ENTER ITEM TO SWAP";
    public const string SwapSuccessfulText = "OK I've now got ";
    public const string CannotOpenText = "How can you open ";
    public const string EatFoodText = "YUM YUM";
    public const string NoTinOpenerText = "Sorry you don't have a can opener.";
    public const string CorrectKeyText =
        "Yes! You have the correct key.\nThe {0} fits.\nRight let's open it...";
    public const string WrongKeyText = "Sorry you don't have the key for the ";
    public const string ContainerContentText = "Inside is ";
    public const string OpenedContainerActionText = "Enter Swap, Pickup or Leave";
    public const string InventoryListForOpenedContainerText = "You hold";
    public const string InventoryEmptyForOpenedContainerText = "You have nothing to swap.";
    public const string InventorySwapForOpenedContainerPrompt = "Which item to swap?";

    private readonly IOutputDevice outputdevice;
    private readonly IInputDevice inputDevice;
    private readonly IMap map;
    private readonly IPlayer player;
    private readonly ISuspense suspense;

    public ThreeSlotInventory(
        IOutputDevice outputdevice,
        IInputDevice inputDevice,
        IMap map,
        IPlayer player,
        ISuspense suspense)
    {
        this.outputdevice = outputdevice;
        this.inputDevice = inputDevice;
        this.map = map;
        this.player = player;
        this.suspense = suspense;
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
            Show();
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
            Show();
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
            $"{PickupConfirmedText} {ItemDescriptionWithoutArticle(item)}.");
    }

    private string ItemDescriptionWithoutArticle(int item)
    {
        return map.ItemDescription(item)[2..];
    }

    private int? FreeSlot()
    {
        for (var i = 0; i < Content.Length; i++)
            if (!Content[i].HasValue) return i;

        return null;
    }

    private void Show()
    {
        if (Empty()) return;

        outputdevice.ShowMessage(YouHoldText);
        List();
    }

    private void List()
    {
        for (var i = 0; i < Content.Length; i++)
        {
            if (Content[i] != null)
            {
                outputdevice.ShowMessage($"{i + 1}. {ItemDescription(i)}");
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

    public bool Swap()
    {
        var itemInCurrentLocation = map.ItemInCurrentLocation();
        if (itemInCurrentLocation < 6)
            outputdevice.ShowMessage(ObjectTooHeavyText);
        else if (Empty())
            outputdevice.ShowMessage(NothingToSwapText);
        else
        {
            Show();
            SwapItems(itemInCurrentLocation);
        }

        return false;
    }

    private void SwapItems(int itemInCurrentLocation)
    {
        bool itemSwapped = false;
        while (!itemSwapped)
        {
            var itemToDrop = inputDevice.ChooseListItem(SwapItemSelectionText);
            if (itemToDrop >= 1 && itemToDrop <= 3 && Content[itemToDrop - 1].HasValue)
            {
                map.AddItemToCurrentLocation(Content[itemToDrop - 1].Value);
                Content[itemToDrop - 1] = itemInCurrentLocation;
                itemSwapped = true;
                outputdevice.ShowMessage(
                    $"{SwapSuccessfulText}{ItemDescription(itemToDrop - 1)}");
            }
        }
    }

    private string ItemDescription(int slot)
    {
        return map.ItemDescription(Content[slot].Value);
    }

    public bool Open()
    {
        if (map.ItemInCurrentLocation() > 7)
            outputdevice.ShowMessage(
                $"{CannotOpenText}{map.ItemInCurrentLocationDescription()}?");
        else if (map.ItemInCurrentLocation() == ItemCollection.TinOfFood)
        {
            if (IsHolding(ItemCollection.TinOpener))
            {
                outputdevice.ShowMessage(EatFoodText);
                player.Restore(60);
                map.RemoveItemFromCurrentLocation();
            }
            else
                outputdevice.ShowMessage(NoTinOpenerText);
        }
        else
        {
            var containers = map.Containers();
            var matchingKey = MatchingKey(containers);
            if (matchingKey >= 0)
            {
                outputdevice.ShowMessage(string.Format(CorrectKeyText, ItemDescriptionWithoutArticle(matchingKey)));
                suspense.Delay(3000);
                outputdevice.Clear();
                outputdevice.ShowMessage($"{ContainerContentText}{map.ItemDescription(UnlockedContainerContent())}");
                var isContainerActioned = false;
                while (!isContainerActioned)
                {
                    outputdevice.ShowMessage(OpenedContainerActionText);
                    var action = inputDevice.ReadCommand();
                    if (action == CommandPalette.Swap || action == CommandPalette.PickUp)
                    {
                        outputdevice.ShowMessage(InventoryListForOpenedContainerText);
                        List();
                        if (action == CommandPalette.PickUp && Full())
                            outputdevice.ShowMessage(InventoryFullForContainerItemText);
                        else if (action == CommandPalette.Swap)
                        {
                            var itemSwapped = false;
                            while (!itemSwapped)
                            {
                                var selectedItem = inputDevice
                                    .ChooseListItem(InventorySwapForOpenedContainerPrompt);
                                if (selectedItem > 0
                                    && selectedItem <= 3
                                    && Content[selectedItem - 1] != null)
                                {
                                    var inventoryItem = Content[selectedItem - 1];
                                    Content[selectedItem - 1] = map.ContainerContent().ItemLocations().ElementAt(map.ItemInCurrentLocation());
                                    map.ContainerContent().UpdateItem(map.ItemInCurrentLocation(), inventoryItem.Value);
                                    itemSwapped = true;
                                    isContainerActioned = true;
                                }
                            }
                        }
                        else if (action == CommandPalette.PickUp)
                        {
                            Insert(map.ContainerContent().ItemLocations().ElementAt(map.ItemInCurrentLocation()).Value);
                            isContainerActioned = true;
                        }
                    }
                    else
                        isContainerActioned = true;
                }
            }
            else
                outputdevice.ShowMessage(
                    $"{WrongKeyText}{ItemDescriptionWithoutArticle(map.ItemInCurrentLocation())}.");
        }

        return false;
    }

    private int UnlockedContainerContent()
    {
        return map.ContainerContent().ItemLocations().ElementAt(map.ItemInCurrentLocation()).Value;
    }

    private int MatchingKey(IEnumerable<int?> containers)
    {
        var itemInCurrentLocation = map.ItemInCurrentLocation();

        if (MatchingLock(containers, 0) == itemInCurrentLocation) return Content[0].Value;
        if (MatchingLock(containers, 1) == itemInCurrentLocation) return Content[1].Value;
        if (MatchingLock(containers, 2) == itemInCurrentLocation) return Content[2].Value;

        return -1;
    }

    private int? MatchingLock(IEnumerable<int?> containers, int slot)
    {
        if (!Content[slot].HasValue ||
            Content[slot].Value < 10 ||
            Content[slot].Value > 16)
            return null;

        return containers.ElementAt(Content[slot].Value - 10);
    }

    public bool IsHolding(int item)
    {
        return Content.Any(_ => _.HasValue && _.Value == item);
    }

    public int Item(int slot)
    {
        return Content[slot].Value;
    }

    public int? Index(int item)
    {
        for (var i = 0; i < Size(); i++)
            if (Content[i] == item)
                return i;

        return null;
    }

    public int Size()
    {
        return Content.Length;
    }

    public void Remove(int index)
    {
        Content[index] = null;
    }
}
