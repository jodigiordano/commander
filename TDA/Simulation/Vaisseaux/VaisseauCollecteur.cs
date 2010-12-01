namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Visuel;
    using Core.Physique;

    class VaisseauCollecteur : VaisseauDoItYourself
    {
        public new bool Actif { get; set; }

        public VaisseauCollecteur(Simulation simulation)
            : base(simulation)
        {
            Representation = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("Collecteur"), Vector3.Zero, Simulation.Scene);
            Representation.Origine = Representation.Centre;
            Representation.Taille = 4;

            RotationMaximaleRad = 0.2f;
            PrixAchat = 0;
            CadenceTir = double.NaN;
            TempsActif = double.MaxValue;
            Actif = true;
        }
    }
}
