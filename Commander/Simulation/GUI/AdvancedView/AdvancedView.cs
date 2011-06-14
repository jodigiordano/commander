﻿namespace EphemereGames.Commander
{
    using System.Collections.Generic;


    class AdvancedView
    {
        private bool visible;
        private Simulation Simulation;
        private EnemiesLives EnemiesLives;
        private List<CelestialBody> CelestialBodies;


        public AdvancedView(Simulation simulation, List<Enemy> enemies, List<CelestialBody> celestialBodies)
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
                    {
                        if (turret.RangeEffect != null)
                            turret.RangeEffect.TerminatedOverride = true;

                        turret.ShowRange = value;
                        turret.RangeImage.Alpha = turret.RangeAlpha;
                    }
            }
        }


        //public void Show()
        //{

        //}


        //public void Hide()
        //{

        //}


        public void Draw()
        {
            if (!Visible)
                return;

            EnemiesLives.Draw();
        }
    }
}