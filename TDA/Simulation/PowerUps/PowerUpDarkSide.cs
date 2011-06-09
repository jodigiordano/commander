namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physique;
    using Microsoft.Xna.Framework;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework.Graphics;


    class PowerUpDarkSide : PowerUp
    {
        public CorpsCeleste CorpsCeleste;


        public PowerUpDarkSide(Simulation simulation)
            : base(simulation)
        {
            Type = PowerUpType.DarkSide;
            Category = PowerUpCategory.Other;
            BuyImage = "darkside";
            BuyPrice = 500;
            BuyTitle = "Dark side (" + BuyPrice + "M$)";
            BuyDescription = "Enemies are hit by a mysterious force when they go behind a planet.";
            NeedInput = false;
            Position = Vector3.Zero;

            CorpsCeleste = new CorpsCeleste(
                    Simulation,
                    "",
                    Vector3.Zero,
                    Vector3.Zero,
                    0,
                    0,
                    new Image("PixelBlanc"),
                    0,
                    0,
                    true,
                    0);
            CorpsCeleste.AttackPoints = 5f;
        }


        public override bool Terminated
        {
            get { return TerminatedOverride; }
        }


        public override void Start()
        {
            EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxDarkSide");
        }
    }
}
