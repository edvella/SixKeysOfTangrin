using System.Collections.Generic;

namespace SixKeysOfTangrin
{
    public interface IMap
    {
        int LocationCount { get; }
        int MaxExitsPerLocation { get; }
        int PlayerLocation { get; set; }

        int? DestinationLocation(int x, int y);
        void Initialise();
        ItemCollection Items();
        IEnumerable<int?> JKeyCollection();
        IEnumerable<int?> KeyCollection();
        string LookCommand();
        void PlaceTwoWayConnection(int x, int y, int destination, bool fixedLocation = false);
        string VisibleItem();
        string ItemDescription(int index);
        int ItemInCurrentLocation();
        string CurrentExits();
        bool GoNorth();
        bool GoEast();
        bool GoUp();
        bool GoSouth();
        bool GoWest();
        bool GoDown();
        void RemoveItemFromCurrentLocation();
        void AddItemToCurrentLocation(int item);
    }
}