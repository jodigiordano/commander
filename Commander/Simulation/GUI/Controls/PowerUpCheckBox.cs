namespace EphemereGames.Commander.Simulation
{
    class PowerUpCheckBox : ImageCheckBox
    {
        public PowerUpType PowerUp;


        public PowerUpCheckBox(string label, PowerUpType type)
            : base(label)
        {
            PowerUp = type;
        }
    }
}
