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
        private bool actif;
        public override bool Actif
        {
            get
            {
                return actif;
            }
        }

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

            actif = true;
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheRetour, Simulation.Main.JoueursConnectes[0].Manette, Simulation.Scene.Nom))
                actif = false;
        }
    }
}
