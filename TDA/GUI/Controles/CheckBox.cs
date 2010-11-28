namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Visuel;
    using Core.Physique;
    using Core.Utilities;
    
    class CheckBox : DrawableGameComponent
    {
        private Simulation Simulation;
        private Curseur Curseur;
        private IVisible Box;
        private IVisible CheckedRep;
        private Cercle BoxCercle;
        private Vector3 Position;

        public bool Checked;

        public CheckBox(Simulation simulation, Curseur curseur, Vector3 position, float priorite)
            : base(simulation.Main)
        {
            Simulation = simulation;
            Curseur = curseur;
            Position = position;

            Box = new IVisible(Core.Persistance.Facade.recuperer<Texture2D>("emplacement"), Position, Simulation.Scene);
            Box.PrioriteAffichage = priorite;
            Box.Origine = Box.Centre;

            CheckedRep = new IVisible("X", Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"), Color.White, Position, Simulation.Scene);
            CheckedRep.Taille = 2;
            CheckedRep.PrioriteAffichage = priorite;
            CheckedRep.Origine = CheckedRep.Centre;

            BoxCercle = new Cercle(Position, 16);
        }

        public override void Update(GameTime gameTime)
        {
            if (Core.Input.Facade.estPeseeUneSeuleFois(Preferences.toucheSelection, Simulation.Main.JoueursConnectes[0].Manette, Simulation.Scene.Nom) &&
                Core.Physique.Facade.collisionCercleCercle(Curseur.Cercle, BoxCercle))
            {
                Checked = !Checked;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Simulation.Scene.ajouterScenable(Box);

            if (Checked)
                Simulation.Scene.ajouterScenable(CheckedRep);
        }
    }
}
