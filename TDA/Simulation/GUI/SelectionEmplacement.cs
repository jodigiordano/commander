
namespace TDA
{
    using System;
    using System.Collections.Generic;
    using RainingSundays.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using RainingSundays.Core.Utilities;
    using ProjectMercury.Emitters;

    class SelectionEmplacement : DrawableGameComponent
    {
        private Emplacement emplacement;
        public Emplacement Emplacement
        {
            get { return emplacement; }
            set
            {
                emplacement = value;

                if (emplacement == null)
                    return;

                Selection.Couleur = emplacement.representation.Couleur;
            }
        }

        private Scene Scene;
        private IVisible Selection;


        public SelectionEmplacement(Simulation simulation)
            : base(simulation.Main)
        {
            Scene = simulation.Scene;

            Selection = new IVisible(RainingSundays.Core.Persistance.Facade.recuperer<Texture2D>("Cercle"), Vector3.Zero, Scene);
            Selection.PrioriteAffichage = 0.94f;
            Selection.Taille = 0.2f;
            Selection.Origine = Selection.Centre;
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (emplacement == null)
                return;

            Selection.Position = Emplacement.Position;

            Scene.ajouterScenable(Selection);
        }
    }
}
