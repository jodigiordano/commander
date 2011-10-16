namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;


    class PowerUpCheckBox : ImageCheckBox
    {
        public PowerUpType PowerUp;


        public PowerUpCheckBox(string label, PowerUpType type, float sizeX)
            : base(new Image(label) { SizeX = sizeX })
        {
            PowerUp = type;
        }
    }
}
