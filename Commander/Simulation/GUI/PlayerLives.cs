namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;


    class PlayerLives
    {
        private Simulator Simulator;
        private CelestialBody CelestialBody;
        private List<Moon> Moons;


        public PlayerLives(Simulator simulator, CelestialBody celestialBody, Color color)
        {
            Simulator = simulator;
            CelestialBody = celestialBody;

            Moons = new List<Moon>();

            if (CelestialBody == null)
                return;


            CelestialBody.Lunes.Clear();

            for (int i = 0; i < celestialBody.LifePoints; i++)
                Moons.Add(CreateMoon());
        }


        public void Update(GameTime gameTime)
        {
            if (CelestialBody == null)
                return;

            int difference = (int) (Moons.Count - CelestialBody.LifePoints);

            if (difference < 0)
            {
                for (int i = 0; i < Math.Abs(difference); i++)
                {
                    Moon m = CreateMoon();
                    Moons.Add(m);
                }
            }

            else if (difference > 0)
            {
                for (int i = 0; i < difference && Moons.Count > 0; i++)
                {
                    Moon m = Moons[Moons.Count - 1];
                    Moons.RemoveAt(Moons.Count - 1);
                }
            }

            foreach (var lune in Moons)
                lune.Update();
        }


        public void Draw()
        {
            if (CelestialBody == null)
                return;

            if (CelestialBody.Alive)
                foreach (var moon in Moons)
                    moon.Draw();
        }


        private Moon CreateMoon()
        {
            Moon lune;

            if (Main.Random.Next(0, 2) == 0)
                lune = new MoonMatrix(Simulator, CelestialBody, 255, "luneVies", 3);
            else
                lune = new MoonPath(Simulator, CelestialBody, 255, "luneVies", 3);

            return lune;
        }
    }
}
