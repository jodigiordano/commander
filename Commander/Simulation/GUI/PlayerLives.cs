namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;


    class PlayerLives
    {
        private CelestialBody celestialBody;

        private Simulator Simulator;
        private List<Moon> Moons;


        public PlayerLives(Simulator simulator)
        {
            Simulator = simulator;

            Moons = new List<Moon>();

            if (CelestialBody == null)
                return;

            CelestialBody.Moons.Clear();
        }


        public CelestialBody CelestialBody
        {
            get { return celestialBody; }
            set
            {
                if (value != null && value != celestialBody)
                    Moons.Clear();

                celestialBody = value;

                if (value != null)
                    foreach (var moon in Moons)
                        moon.CelestialBody = value;
            }
        }


        public void Update()
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
