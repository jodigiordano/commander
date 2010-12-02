namespace TDA
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

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

            for (int i = 0; i < celestialBody.PointsVie; i++)
                Moons.Add(CreateMoon());
        }


        public void Update(GameTime gameTime)
        {
            int difference = (int) (Moons.Count - CelestialBody.PointsVie);

            if (difference < 0)
            {
                for (int i = 0; i < Math.Abs(difference); i++)
                    Moons.Add(CreateMoon());
            }

            else if (difference > 0)
            {
                for (int i = 0; i < difference && Moons.Count > 0; i++)
                    Moons.RemoveAt(Moons.Count - 1);
            }

            foreach (var lune in Moons)
                lune.Update(gameTime);
        }


        public void Draw()
        {
            if (CelestialBody.EstVivant)
            {
                foreach (var lune in Moons)
                    lune.Draw(null);
            }
        }


        private Lune CreateMoon()
        {
            Lune lune;

            if (Main.Random.Next(0, 2) == 0)
                lune = new LuneMatrice(Simulation, CelestialBody);
            else
                lune = new LuneTrajet(Simulation, CelestialBody);

            lune.Representation.Texture = Core.Persistance.Facade.recuperer<Texture2D>("luneVies");
            lune.Representation.Couleur.A = 255;
            lune.Representation.Taille = 3;
            lune.Representation.Origine = lune.Representation.Centre;

            return lune;
        }
    }
}
