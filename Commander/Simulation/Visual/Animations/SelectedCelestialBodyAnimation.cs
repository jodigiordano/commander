namespace EphemereGames.Commander.Simulation.Player
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

                if (Simulator.EditorEditingMode)
                {
                    if (celestialBody != null && value == null)
                        celestialBody.ShowPath = false;
                    else if (celestialBody == null && value != null)
                        value.ShowPath = true;
                }

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

            Vector3 displacement;
            Vector3.Subtract(ref CelestialBody.position, ref PositionLastEmission, out displacement);

            if (displacement != Vector3.Zero)
                Selection.Move(ref displacement);

            Selection.Trigger(ref CelestialBody.position);
            PositionLastEmission = CelestialBody.position;
        }
    }
}
