namespace SixKeysOfTangrin
{
    public interface IInventory
    {
        bool Dump();
        bool PickUp();
        bool Swap();
        bool Open();
    }
}
