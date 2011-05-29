namespace EphemereGames.Commander
{
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
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
            Selection.VisualPriority = Preferences.PrioriteGUISelectionCorpsCeleste;

            WidgetName = new IVisible
            (
                "",
                EphemereGames.Core.Persistance.Facade.GetAsset<SpriteFont>("Pixelite"),
                Color.White,
                new Vector3(Selection.Position.X, Selection.Position.Y + ((CircleEmitter)Selection.ParticleEffect[0]).Radius + 50, 0)
            );
            WidgetName.Taille = 2;
            WidgetName.VisualPriority = Preferences.PrioriteGUISelectionCorpsCeleste;
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

                ((CircleEmitter)Selection.ParticleEffect[0]).Radius = celestialBody.Cercle.Radius + 5;
            }
        }


        public void Update(GameTime gameTime)
        {
            if (CelestialBody == null || !CelestialBody.Alive)
                return;

            Vector3 deplacement;
            Vector3.Subtract(ref CelestialBody.position, ref PositionLastEmission, out deplacement);

            if (deplacement.X != 0 && deplacement.Y != 0)
                Selection.Deplacer(ref deplacement);

            Selection.Emettre(ref CelestialBody.position);
            PositionLastEmission = CelestialBody.position;
        }


        public void Draw()
        {
            if (CelestialBody == null || !CelestialBody.Alive)
                return;
        }
    }
}
