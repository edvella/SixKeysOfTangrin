namespace SixKeysOfTangrin;

public class ContainerContent : ItemCollection
{
    public ContainerContent() : base(7, 10, 7)
    {
    }

    public override void ScatterAroundMap()
    {
        base.ScatterAroundMap();
        itemLocations[5] = Treasure;
    }
}
