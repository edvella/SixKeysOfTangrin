using System;
using System.Collections.Generic;
using System.Linq;

namespace SixKeysOfTangrin
{
    public class ItemCollection
    {
        public const int Nothing = 29;

        private readonly Random rnd = new(DateTime.Now.Second);

        private readonly int itemCount;
        private readonly int maxLocation;
        private readonly int locationOffset;

        protected int?[] itemLocations;

        public IEnumerable<int?> ItemLocations() { return itemLocations; }

        public ItemCollection() : this(30, 0, 31)
        {

        }

        public ItemCollection(int itemCount, int locationOffset, int maxLocation)
        {
            this.itemCount = itemCount;
            this.locationOffset = locationOffset;
            this.maxLocation = maxLocation;
            itemLocations = new int?[itemCount];
        }

        public virtual void ScatterAroundMap()
        {
            for (int i = 0; i < itemCount; i++)
            {
                PlaceItemAtRandom(i);
            }
        }

        public void PlaceItem(int i, int location)
        {
            if (!itemLocations.Any(_ => _.HasValue && _.Value == location) && location != 27)
                itemLocations[i] = location;
            else
                PlaceItemAtRandom(i);
        }

        private void PlaceItemAtRandom(int item)
        {
            PlaceItem(item, rnd.Next(maxLocation) + locationOffset);
        }

        public void UpdateItem(int index, int location)
        {
            itemLocations[index] = location;
        }
    }
}
