namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;


    class PlanetarySystemController
    {
        public List<CorpsCeleste> CelestialBodies;
        public event PhysicalObjectHandler ObjectDestroyed;
        public Path Path;
        public Path PathPreview;

        private bool demoMode;


        public PlanetarySystemController(Simulation simulation)
        {
            CelestialBodies = new List<CorpsCeleste>();
            Path = new Path(simulation, new Color(255, 255, 255, 100), TypeBlend.Add);
            PathPreview = new Path(simulation, new Color(255, 255, 255, 25), TypeBlend.Add);
            DemoMode = false;
        }


        public void Initialize()
        {
            Path.CorpsCelestes = CelestialBodies;
            Path.Initialize();

            PathPreview.CorpsCelestes = CelestialBodies;
            PathPreview.Initialize();
        }



        public bool DemoMode
        {
            get { return demoMode; }
            set
            {
                demoMode = value;
            }
        }


        public void Update(GameTime gameTime)
        {
            // Pour les corps celestes qui meurent par eux-memes
            for (int i = CelestialBodies.Count - 1; i > -1; i--)
                if (!CelestialBodies[i].Alive)
                {
                    NotifyObjetDetruit(CelestialBodies[i]);

                    if (Path.contientCorpsCeleste(CelestialBodies[i]))
                    {
                        Path.enleverCorpsCeleste(CelestialBodies[i]);
                        PathPreview.enleverCorpsCeleste(CelestialBodies[i]);
                    }

                    CelestialBodies.RemoveAt(i);
                }

            for (int i = 0; i < CelestialBodies.Count; i++)
                CelestialBodies[i].Update(gameTime);

            Path.Update(gameTime);
            PathPreview.Update(gameTime);
        }


        public void Draw()
        {
            for (int i = 0; i < CelestialBodies.Count; i++)
                CelestialBodies[i].Draw(null);
        }


        public bool AddTurret(Turret turret)
        {
            CorpsCeleste celestialBody = turret.CelestialBody;

            celestialBody.Turrets.Add(turret);

            if (turret.Type == TurretType.Gravitational &&
                celestialBody.Priorite != -1 &&
                !Path.contientCorpsCeleste(celestialBody))
            {
                Path.ajouterCorpsCeleste(celestialBody);
                PathPreview.ajouterCorpsCeleste(celestialBody);
            }

            return true;
        }


        public bool RemoveTurret(Turret turret)
        {
            CorpsCeleste celestialBody = turret.CelestialBody;

            int nbGravTurrets = 0;

            foreach (var turret2 in celestialBody.Turrets)
                if (turret2.Type == TurretType.Gravitational)
                    nbGravTurrets++;

            if (turret.Type == TurretType.Gravitational && nbGravTurrets == 1)
            {
                Path.enleverCorpsCeleste(celestialBody);
                PathPreview.enleverCorpsCeleste(celestialBody);
            }

            celestialBody.Turrets.Remove(turret);

            return true;
        }


        public void DoDestroyCelestialBody(IObjetPhysique physicalObject)
        {
            var celestialBody = (CorpsCeleste) physicalObject;

            celestialBody.DoDie();

            Path.enleverCorpsCeleste(celestialBody);
            PathPreview.enleverCorpsCeleste(celestialBody);

            CelestialBodies.Remove(celestialBody);

            NotifyObjetDetruit(celestialBody);
        }


        private void NotifyObjetDetruit(IObjetPhysique obj)
        {
            if (ObjectDestroyed != null)
                ObjectDestroyed(obj);
        }
    }
}
