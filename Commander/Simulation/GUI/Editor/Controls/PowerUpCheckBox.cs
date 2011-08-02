namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;


    class PowerUpCheckBox : ImageCheckBox
    {
        public PowerUpType PowerUp;


        public PowerUpCheckBox(string label, PowerUpType type)
            : base(new Image(label) { SizeX = 6 })
        {
            PowerUp = type;
        }
    }
}
