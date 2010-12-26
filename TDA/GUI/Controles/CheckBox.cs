namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using EphemereGames.Core.Visuel;
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Utilities;
    
    class CheckBox : DrawableGameComponent
    {
        private Simulation Simulation;
        private Cursor Curseur;
        private IVisible Box;
        private IVisible CheckedRep;
        private Cercle BoxCercle;
        private Vector3 Position;

        public bool Checked;

        public CheckBox(Simulation simulation, Cursor curseur, Vector3 position, float priorite)
            : base(simulation.Main)
        {
            Simulation = simulation;
            Curseur = curseur;
            Position = position;

            Box = new IVisible(EphemereGames.Core.Persistance.Facade.GetAsset<Texture2D>("emplacement"), Position);
            Box.VisualPriority = priorite;
            Box.Origine = Box.Centre;

            CheckedRep = new IVisible("X", EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"), Color.White, Position);
            CheckedRep.Taille = 2;
            CheckedRep.VisualPriority = priorite;
            CheckedRep.Origine = CheckedRep.Centre;

            BoxCercle = new Cercle(Position, 16);
        }


        public void doClick()
        {
            if (EphemereGames.Core.Physique.Facade.collisionCercleCercle(Curseur.Cercle, BoxCercle))
                Checked = !Checked;
        }


        public override void Draw(GameTime gameTime)
        {
            Simulation.Scene.ajouterScenable(Box);

            if (Checked)
                Simulation.Scene.ajouterScenable(CheckedRep);
        }
    }
}
