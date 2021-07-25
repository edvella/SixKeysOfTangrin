namespace SixKeysOfTangrin
{
    public interface IInventory
    {
        bool Dump();
        bool PickUp();
        bool Swap();
        bool Open();
        bool IsHolding(int item);
        int Item(int slot);
        int? Index(int item);
        int Size();
        void Remove(int index);
    }
}
