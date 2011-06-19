namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using ParallelTasks;


    class PlanetarySystemController
    {
        public List<CelestialBody> CelestialBodies;
        public List<ShootingStar> ShootingStars;
        public event PhysicalObjectHandler ObjectDestroyed;
        public Path Path;
        public Path PathPreview;

        private Simulator Simulator;
        private Core.Utilities.Pool<ShootingStar> ShootingStarsFactory;
        private bool DeadlyShootingStars;

        private Action SyncUpdateShootingStars;
        private Action SyncPathPreview;

        private Particle Stars;
        private double StarsEmitter;

        private bool FinalSolution;


        public PlanetarySystemController(Simulator simulator)
        {
            Simulator = simulator;
            CelestialBodies = new List<CelestialBody>();
            ShootingStars = new List<ShootingStar>();
            ShootingStarsFactory = new Core.Utilities.Pool<ShootingStar>();
            Path = new Path(simulator, new Color(255, 255, 255, 100), TypeBlend.Add);
            PathPreview = new Path(simulator, new Color(255, 255, 255, 0), TypeBlend.Add);
        }


        public void Initialize()
        {
            Path.CelestialBodies = CelestialBodies;
            Path.Initialize();

            PathPreview.CelestialBodies = CelestialBodies;
            PathPreview.Initialize();

            SyncUpdateShootingStars = new Action(UpdateShootingStars);
            SyncPathPreview = new Action(PathPreview.Update);

            Stars = Simulator.Scene.Particles.Get(@"etoilesScintillantes");
            Stars.VisualPriority = Preferences.PrioriteGUIEtoiles;
            StarsEmitter = 0;
            DeadlyShootingStars = false;
            FinalSolution = false;
        }


        public void Update(GameTime gameTime)
        {
            for (int i = CelestialBodies.Count - 1; i > -1; i--)
            {
                CelestialBody c = CelestialBodies[i];

                if (!c.Alive)
                {
                    c.DoDie();

                    if (Path.contientCorpsCeleste(c))
                    {
                        Path.enleverCorpsCeleste(c);
                        PathPreview.enleverCorpsCeleste(c);
                    }

                    CelestialBodies.RemoveAt(i);

                    NotifyObjetDetruit(c);
                }
            }


            Task t1, t2, t3;

            t1 = Parallel.Start(SyncUpdateShootingStars);
            t2 = Parallel.Start(SyncPathPreview);

            UpdateCelestialBodies();
            Path.Update();

            t1.Wait();
            t2.Wait();

            StarsEmitter += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (StarsEmitter >= 100)
            {
                Vector2 v2 = Vector2.Zero;
                Stars.Trigger(ref v2);
                StarsEmitter = 0;
            }
        }


        //public void Show()
        //{
        //    for (int i = 0; i < CelestialBodies.Count; i++)
        //        CelestialBodies[i].Show();
        //}


        //public void Hide()
        //{
        //    for (int i = 0; i < CelestialBodies.Count; i++)
        //        CelestialBodies[i].Hide();
        //}


        public void Draw()
        {
            foreach (var c in CelestialBodies)
                c.Draw();
        }


        public bool AddTurret(Turret turret)
        {
            CelestialBody celestialBody = turret.CelestialBody;

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
            CelestialBody celestialBody = turret.CelestialBody;

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


        public void DoPowerUpStarted(PowerUp powerUp)
        {
            if (powerUp.Type == PowerUpType.Pulse)
                ((PowerUpPulse) powerUp).Path = Path;

            if (powerUp.Type == PowerUpType.DarkSide)
                foreach (var c in CelestialBodies)
                    c.DarkSide = true;

            if (powerUp.Type == PowerUpType.DeadlyShootingStars)
            {
                DeadlyShootingStars = true;

                Vector3 newColor = Color.Red.ToVector3();
                Vector4 newColor2 = Color.Red.ToVector4();

                var stars = Stars.ParticleEffect[0];

                stars.ReleaseColour = newColor;

                for (int i = 0; i < stars.ActiveParticlesCount; i++)
                {
                    var star = stars.Particles[i];

                    star.Colour = newColor2;

                    var effect = Simulator.Scene.Particles.Get(@"starExplosion");
                    effect.ParticleEffect[0].ReleaseColour = newColor;
                    effect.Trigger(ref star.Position);
                    effect.VisualPriority = Preferences.PrioriteFondEcran - 0.00001f;
                    Simulator.Scene.Particles.Return(effect);
                }
            }

            if (powerUp.Type == PowerUpType.FinalSolution)
                FinalSolution = true;
        }


        public void DoPowerUpStopped(PowerUp powerUp)
        {
            if (powerUp.Type != PowerUpType.FinalSolution)
                return;

            FinalSolution = false;

            PowerUpLastSolution p = (PowerUpLastSolution) powerUp;

            if (!p.GoAhead)
                return;

            p.CelestialBody.ZoneImpactDestruction = p.ZoneImpactDestruction;
            p.CelestialBody.AttackPoints = p.AttackPoints;
            p.CelestialBody.LifePoints = 0;
        }


        private void NotifyObjetDetruit(IObjetPhysique obj)
        {
            if (ObjectDestroyed != null)
                ObjectDestroyed(obj);
        }


        private void UpdateShootingStars()
        {
            if (Main.Random.Next(0, DeadlyShootingStars ? 300 : 1000) == 0)
            {
                ShootingStar ss = ShootingStarsFactory.Get();
                ss.Scene = Simulator.Scene;
                ss.Terrain = Simulator.Terrain;
                ss.LoadContent();
                ss.Initialize();

                ShootingStars.Add(ss);
            }

            for (int i = ShootingStars.Count - 1; i > -1; i--)
            {
                ShootingStar ss = ShootingStars[i];

                ss.Update();

                if (!ss.Alive)
                {
                    ShootingStarsFactory.Return(ss);
                    ShootingStars.RemoveAt(i);
                }
            }
        }


        private void UpdateCelestialBodies()
        {
            foreach (var c in CelestialBodies)
                c.Update();
        }
    }
}
