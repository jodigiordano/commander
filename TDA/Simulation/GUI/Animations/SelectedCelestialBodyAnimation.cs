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
        private Particle Selection;
        private Vector3 PositionLastEmission;


        public SelectedCelestialBodyAnimation(Simulation simulation)
        {
            Simulation = simulation;

            Selection = Simulation.Scene.Particules.Get("selectionCorpsCeleste");
            Selection.VisualPriority = Preferences.PrioriteGUISelectionCorpsCeleste;
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

                ((CircleEmitter)Selection.ParticleEffect[0]).Radius = celestialBody.Circle.Radius + 5;
            }
        }


        public void Draw()
        {
            if (CelestialBody == null || !CelestialBody.Alive)
                return;

            Vector3 deplacement;
            Vector3.Subtract(ref CelestialBody.position, ref PositionLastEmission, out deplacement);

            if (deplacement.X != 0 && deplacement.Y != 0)
                Selection.Move(ref deplacement);

            Selection.Trigger(ref CelestialBody.position);
            PositionLastEmission = CelestialBody.position;
        }
    }
}
