namespace SixKeysOfTangrin
{
    public class JKeyCollection : ItemCollection
    {
        public JKeyCollection() : base(6, 10, 7)
        {
        }

        public override void ScatterAroundMap()
        {
            base.ScatterAroundMap();
            itemLocations[5] = 28;
        }
    }
}
