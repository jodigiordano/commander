namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Audio;
    using Microsoft.Xna.Framework;


    class PowerUpDarkSide : PowerUp
    {
        public CelestialBody CorpsCeleste;


        public PowerUpDarkSide(Simulator simulator)
            : base(simulator)
        {
            Type = PowerUpType.DarkSide;
            Category = PowerUpCategory.Other;
            BuyImage = "darkside";
            BuyPrice = 500;
            BuyTitle = "Dark side (" + BuyPrice + "M$)";
            BuyDescription = "Enemies are hit by a mysterious force when they go behind a planet.";
            NeedInput = false;
            Position = Vector3.Zero;

            CorpsCeleste = new CelestialBody(
                    Simulation,
                    "",
                    Vector3.Zero,
                    Vector3.Zero,
                    0,
                    Size.Small,
                    float.MaxValue,
                    null,
                    0,
                    0);
            CorpsCeleste.AttackPoints = 0.5f;
        }


        public override bool Terminated
        {
            get { return TerminatedOverride; }
        }


        public override void Start()
        {
            Audio.PlaySfx(@"sfxDarkSide");
        }
    }
}
