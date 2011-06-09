namespace EphemereGames.Commander
{
    using System.Collections.Generic;


    class AdvancedView
    {
        private bool visible;
        private Simulation Simulation;
        private EnemiesLives EnemiesLives;
        private List<CorpsCeleste> CelestialBodies;


        public AdvancedView(Simulation simulation, List<Enemy> enemies, List<CorpsCeleste> celestialBodies)
        {
            Simulation = simulation;
            visible = false;
            CelestialBodies = celestialBodies;
            EnemiesLives = new EnemiesLives(simulation, enemies);
        }


        public bool Visible
        {
            get { return visible; }
            set
            {
                visible = value;

                foreach (var celestialBody in CelestialBodies)
                    foreach (var turret in celestialBody.Turrets)
                        turret.ShowRange = value;
            }
        }


        public void Show()
        {

        }


        public void Hide()
        {

        }


        public void Draw()
        {
            if (!Visible)
                return;

            EnemiesLives.Draw();
        }
    }
}
