
namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Core.Utilities;
    using ProjectMercury.Emitters;

    class SelectedCelestialBodyAnimation
    {
        private CorpsCeleste celestialBody;
        private Simulation Simulation;
        private ParticuleEffectWrapper Selection;
        private Vector3 PositionLastEmission;
        private IVisible WidgetName;


        public SelectedCelestialBodyAnimation(Simulation simulation)
        {
            Simulation = simulation;

            Selection = Simulation.Scene.Particules.recuperer("selectionCorpsCeleste");
            Selection.PrioriteAffichage = Preferences.PrioriteGUISelectionCorpsCeleste;

            WidgetName = new IVisible
            (
                "",
                Core.Persistance.Facade.recuperer<SpriteFont>("Pixelite"),
                Color.White,
                new Vector3(Selection.Position.X, Selection.Position.Y + ((CircleEmitter)Selection.ParticleEffect[0]).Radius + 50, 0)
            );
            WidgetName.Taille = 2;
            WidgetName.PrioriteAffichage = Preferences.PrioriteGUISelectionCorpsCeleste;
        }


        public CorpsCeleste CelestialBody
        {
            get { return celestialBody; }
            set
            {
                celestialBody = value;

                if (celestialBody == null)
                    return;

                PositionLastEmission = celestialBody.Position;

                WidgetName.Texte = celestialBody.Nom;
                WidgetName.Origine = WidgetName.Centre;

                ((CircleEmitter)Selection.ParticleEffect[0]).Radius = celestialBody.Cercle.Rayon + 5;
            }
        }


        public void Update(GameTime gameTime)
        {
            if (CelestialBody == null || !CelestialBody.EstVivant)
                return;

            Vector3 deplacement;
            Vector3.Subtract(ref CelestialBody.position, ref PositionLastEmission, out deplacement);

            if (deplacement.X != 0 && deplacement.Y != 0)
                Selection.Deplacer(ref deplacement);

            Selection.Emettre(ref CelestialBody.position);
            PositionLastEmission = CelestialBody.Position;
        }


        public void Draw()
        {
            if (CelestialBody == null || !CelestialBody.EstVivant)
                return;

            if (!Simulation.ModeDemo)
            {
                WidgetName.Position = new Vector3(CelestialBody.Position.X, CelestialBody.Position.Y - ((CircleEmitter)Selection.ParticleEffect[0]).Radius - 13, 0);
                Simulation.Scene.ajouterScenable(WidgetName);
            }
        }
    }
}
