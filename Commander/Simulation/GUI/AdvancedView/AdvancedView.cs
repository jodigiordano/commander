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


        public AdvancedView(Simulator simulator)
        {
            Simulator = simulator;
            EnemiesLives = new EnemiesLives(Simulator);

            visible = false;
        }


        public void Initialize()
        {
            EnemiesLives.Enemies = Enemies;
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
