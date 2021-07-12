namespace SixKeysOfTangrin
{
    public interface IPlayer
    {
        int EnergyLeft();
        string EnergyStatus();
        void Restore(int amount);
        void RestoreFullHealth();
        void Drain(int tideTime);
    }
}
