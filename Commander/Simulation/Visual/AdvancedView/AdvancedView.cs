namespace EphemereGames.Commander.Simulation
{


    class AdvancedView
    {
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


        public bool Visible
        {
            get { return visible; }
            set
            {
                visible = value;

                foreach (var celestialBody in Simulator.Data.Level.PlanetarySystem)
                {
                    foreach (var turret in celestialBody.Turrets)
                    {
                        if (turret.RangeEffect != -1)
                            Simulator.Scene.VisualEffects.CancelCallback(turret.RangeEffect);

                        turret.ShowRange = value;
                        turret.RangeImage.Alpha = turret.RangeAlpha;
                    }

                    celestialBody.ShowPath = value;
                }

                CelestialBodiesPaths.Visible = value;
            }
        }


        public void Draw()
        {
            EnemiesLives.ShowAll = Visible;

            EnemiesLives.Draw();

            if (Visible)
                CelestialBodiesPaths.Draw();
        }
    }
}
