namespace EphemereGames.Commander.Simulation
{
    using System.Collections.Generic;


    class AdvancedView
    {
        private bool visible;
        private Simulator Simulator;
        private EnemiesLives EnemiesLives;
        private List<CelestialBody> CelestialBodies;


        public AdvancedView(Simulator simulator, List<Enemy> enemies, List<CelestialBody> celestialBodies)
        {
            Simulator = simulator;
            visible = false;
            CelestialBodies = celestialBodies;
            EnemiesLives = new EnemiesLives(simulator, enemies);
        }


        public bool Visible
        {
            get { return visible; }
            set
            {
                visible = value;

                foreach (var celestialBody in CelestialBodies)
                    foreach (var turret in celestialBody.Turrets)
                    {
                        if (turret.RangeEffect != null)
                            turret.RangeEffect.TerminatedOverride = true;

                        turret.ShowRange = value;
                        turret.RangeImage.Alpha = turret.RangeAlpha;
                    }
            }
        }


        public void Draw()
        {
            if (!Visible)
                return;

            EnemiesLives.Draw();
        }
    }
}
