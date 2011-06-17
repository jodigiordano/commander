namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class PlayerLives
    {
        private Simulator Simulation;
        private CelestialBody CelestialBody;
        private List<Moon> Moons;


        public PlayerLives(Simulator simulation, CelestialBody celestialBody, Color color)
        {
            Simulation = simulation;
            CelestialBody = celestialBody;

            CelestialBody.Lunes.Clear();

            Moons = new List<Moon>();

            for (int i = 0; i < celestialBody.LifePoints; i++)
                Moons.Add(CreateMoon());
        }


        public void Update(GameTime gameTime)
        {
            int difference = (int) (Moons.Count - CelestialBody.LifePoints);

            if (difference < 0)
            {
                for (int i = 0; i < Math.Abs(difference); i++)
                {
                    Moon m = CreateMoon();
                    //m.Show();
                    Moons.Add(m);
                }
            }

            else if (difference > 0)
            {
                for (int i = 0; i < difference && Moons.Count > 0; i++)
                {
                    Moon m = Moons[Moons.Count - 1];
                    //m.Hide();
                    Moons.RemoveAt(Moons.Count - 1);
                }
            }

            foreach (var lune in Moons)
                lune.Update();
        }


        //public void Show()
        //{
        //    foreach (var moon in Moons)
        //        moon.Show();
        //}


        //public void Hide()
        //{
        //    foreach (var moon in Moons)
        //        moon.Hide();
        //}


        public void Draw()
        {
            if (CelestialBody.Alive)
                foreach (var moon in Moons)
                    moon.Draw();
        }


        private Moon CreateMoon()
        {
            Moon lune;

            if (Main.Random.Next(0, 2) == 0)
                lune = new MoonMatrix(Simulation, CelestialBody, 255, "luneVies", 3);
            else
                lune = new MoonPath(Simulation, CelestialBody, 255, "luneVies", 3);

            return lune;
        }
    }
}
