
namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Utilities;
    using ProjectMercury.Emitters;

    class SelectionCorpsCeleste : DrawableGameComponent
    {
        private CorpsCeleste corpsCeleste;
        public CorpsCeleste CorpsSelectionne
        {
            get { return corpsCeleste; }
            set
            {
                corpsCeleste = value;

                if (corpsCeleste == null)
                    return;

                PositionDerniereEmission = corpsCeleste.Position;

                WidgetNom.Texte = corpsCeleste.Nom;
                WidgetNom.Origine = WidgetNom.Centre;

                ((CircleEmitter)Selection.ParticleEffect[0]).Radius = corpsCeleste.Cercle.Rayon + 5;
            }
        }

        private Scene Scene;
        private ParticuleEffectWrapper Selection;
        private Vector3 PositionDerniereEmission;
        private IVisible WidgetNom;

        public bool ModeDemo = false;


        public SelectionCorpsCeleste(Simulation simulation)
            : base(simulation.Main)
        {
            Scene = simulation.Scene;

            Selection = Scene.Particules.recuperer("selectionCorpsCeleste");
            Selection.PrioriteAffichage = Preferences.PrioriteGUISelectionCorpsCeleste;

            WidgetNom = new IVisible
            (
                "",
                Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"),
                Color.White,
                new Vector3(Selection.Position.X, Selection.Position.Y + ((CircleEmitter)Selection.ParticleEffect[0]).Radius + 50, 0),
                Scene
            );
            WidgetNom.Taille = 2;
            WidgetNom.PrioriteAffichage = Preferences.PrioriteGUISelectionCorpsCeleste;
        }


        public override void Update(GameTime gameTime)
        {
            if (CorpsSelectionne == null || !CorpsSelectionne.EstVivant)
                return;

            Vector3 deplacement;
            Vector3.Subtract(ref CorpsSelectionne.position, ref PositionDerniereEmission, out deplacement);

            if (deplacement.X != 0 && deplacement.Y != 0)
                Selection.Deplacer(ref deplacement);

            Selection.Emettre(ref CorpsSelectionne.position);
            PositionDerniereEmission = CorpsSelectionne.Position;
        }

        public override void Draw(GameTime gameTime)
        {
            if (CorpsSelectionne == null || !CorpsSelectionne.EstVivant)
                return;

            if (!ModeDemo)
            {
                WidgetNom.Position = new Vector3(CorpsSelectionne.Position.X, CorpsSelectionne.Position.Y - ((CircleEmitter)Selection.ParticleEffect[0]).Radius - 13, 0);
                Scene.ajouterScenable(WidgetNom);
            }
        }
    }
}
