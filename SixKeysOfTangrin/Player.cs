namespace SixKeysOfTangrin
{
    public class Player : IPlayer
    {
        public const string Exhausted = "YOU ARE EXTREMELY EXHAUSTED";
        public const string Died = "YOU HAVE BECOME A VICTIM OF THE CAVES ON TANGRIN";
        public const string Weary = "YOU ARE VERY WEARY, AND MUST GET OUT FAST";
        public const string Tired = "YOU ARE GETTING TIRED - YOU SHOULD THINK OF RETURNING";
        public const string Peckish = "YOU ARE STILL QUITE FIT, BUT GETTING PECKISH";
        public const string LessStrong = "YOU ARE NOT QUITE AS STRONG NOW";
        public const string Strong = "YOU ARE STRONG ENOUGH TO TACKLE ANYTHING";
        public const string Efficient = "YOU ARE AT 100% EFFICIENCY";
        public const string FullStrength = "YOU ARE AT MAXIMUM STRENGTH - MAKE THE MOST OF IT";

        public const int FullEnergy = 99;

        public int Energy { get; set; } = FullEnergy;

        public string EnergyStatus()
        {
            if (Energy < 2) return $"{Exhausted}\n{Died}";
            if (Energy < 10) return Exhausted;
            if (Energy < 20) return Weary;
            if (Energy < 30) return Tired;
            if (Energy < 40) return Peckish;
            if (Energy < 50) return null;
            if (Energy < 60) return LessStrong;
            if (Energy < 80) return Strong;
            if (Energy < 90) return Efficient;

            return FullStrength;
        }

        public void RestoreHealth()
        {
            Energy = FullEnergy;
        }
        public void Drain(int tideTime)
        {
            Energy -= (410 - tideTime) / 95;
        }
    }
}
