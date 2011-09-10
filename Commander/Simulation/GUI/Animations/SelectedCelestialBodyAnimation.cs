namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;


    class SelectedCelestialBodyAnimation
    {
        private CelestialBody celestialBody;
        private Simulator Simulator;
        private Particle Selection;
        private Vector3 PositionLastEmission;


        public SelectedCelestialBodyAnimation(Simulator simulator)
        {
            Simulator = simulator;

            Selection = Simulator.Scene.Particles.Get(@"selectionCorpsCeleste");
            Selection.VisualPriority = VisualPriorities.Default.CelestialBodySelection;
        }


        public CelestialBody CelestialBody
        {
            get { return celestialBody; }
            set
            {
                if (celestialBody == null && value != null)
                    PositionLastEmission = value.Position;

                celestialBody = value;

                if (celestialBody == null)
                    return;


                ((CircleEmitter)Selection.Model[0]).Radius = celestialBody.Circle.Radius + 5;
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
