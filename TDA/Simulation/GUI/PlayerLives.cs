namespace EphemereGames.Commander
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;


    class PlayerLives
    {
        private Simulation Simulation;
        private CorpsCeleste CelestialBody;
        private List<Lune> Moons;


        public PlayerLives(Simulation simulation, CorpsCeleste celestialBody, Color color)
        {
            Simulation = simulation;
            CelestialBody = celestialBody;

            CelestialBody.Lunes.Clear();

            Moons = new List<Lune>();

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
                    Lune m = CreateMoon();
                    m.Show();
                    Moons.Add(m);
                }
            }

            else if (difference > 0)
            {
                for (int i = 0; i < difference && Moons.Count > 0; i++)
                {
                    Lune m = Moons[Moons.Count - 1];
                    m.Hide();
                    Moons.RemoveAt(Moons.Count - 1);
                }
            }

            foreach (var lune in Moons)
                lune.Update(gameTime);
        }


        public void Show()
        {
            foreach (var moon in Moons)
                moon.Show();
        }


        public void Hide()
        {
            foreach (var moon in Moons)
                moon.Hide();
        }


        public void Draw()
        {
            if (CelestialBody.Alive)
                foreach (var lune in Moons)
                    lune.Draw();
        }


        private Lune CreateMoon()
        {
            Lune lune;

            if (Main.Random.Next(0, 2) == 0)
                lune = new LuneMatrice(Simulation, CelestialBody, 255);
            else
                lune = new LuneTrajet(Simulation, CelestialBody, 255);

            lune.Representation = new Image("luneVies")
            {
                SizeX = 3
            };

            return lune;
        }
    }
}
