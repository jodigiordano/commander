namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    class AdvancedView
    {
        public List<Enemy> Enemies;
        public List<CelestialBody> CelestialBodies;

        private bool visible;
        private Simulator Simulator;
        private EnemiesLives EnemiesLives;
        private CelestialBodiesPathPreviews CelestialBodiesPaths;


        public AdvancedView(Simulator simulator)
        {
            Simulator = simulator;
            EnemiesLives = new EnemiesLives(Simulator);
            CelestialBodiesPaths = new CelestialBodiesPathPreviews(Simulator);

            visible = false;
        }


        public void Initialize()
        {
            CelestialBodiesPaths.CelestialBodies = CelestialBodies;
            EnemiesLives.Enemies = Enemies;
        }


        public bool Visible
        {
            get { return visible; }
            set
            {
                visible = value;

                foreach (var celestialBody in CelestialBodies)
                {
                    foreach (var turret in celestialBody.Turrets)
                    {
                        if (turret.RangeEffect != null)
                            turret.RangeEffect.TerminatedOverride = true;

                        turret.ShowRange = value;
                        turret.RangeImage.Alpha = turret.RangeAlpha;
                    }

                    celestialBody.ShowPath = value;
                }
            }
        }


        public void Draw()
        {
            if (!Visible)
                return;

            EnemiesLives.Draw();
            CelestialBodiesPaths.Draw();
        }
    }
}
