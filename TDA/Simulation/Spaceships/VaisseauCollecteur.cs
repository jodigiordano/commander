namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Physique;

    class VaisseauCollecteur : VaisseauDoItYourself, PowerUp
    {
        public new bool Actif { get; set; }
        public Vector3 BuyPosition { get; set; }

        public VaisseauCollecteur(Simulation simulation)
            : base(simulation)
        {
            Representation = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("Collecteur"), Vector3.Zero);
            Representation.Origine = Representation.Centre;
            Representation.Taille = 4;

            RotationMaximaleRad = 0.2f;
            PrixAchat = 0;
            CadenceTir = double.NaN;
            TempsActif = double.MaxValue;
            Actif = true;
        }


        public PowerUpType Type
        {
            get { return PowerUpType.Collector; }
        }


        public string BuyImage
        {
            get { return "Collecteur"; }
        }


        public int BuyPrice
        {
            get { return 0; }
        }
    }
}
