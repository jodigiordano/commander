namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    class AdvancedView
    {
        private bool visible;
        private Simulation Simulation;
        private EnemiesLives EnemiesLives;
        private List<CorpsCeleste> CelestialBodies;


        public AdvancedView(Simulation simulation, List<Ennemi> enemies, List<CorpsCeleste> celestialBodies)
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


        public void Draw()
        {
            if (!Visible)
                return;

            EnemiesLives.Draw();
        }
    }
}
