namespace SixKeysOfTangrin
{
    public interface IPlayer
    {
        string EnergyStatus();
        void RestoreHealth();
        void Drain(int tideTime);
    }
}
