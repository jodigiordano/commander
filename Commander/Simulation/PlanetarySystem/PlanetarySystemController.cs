﻿namespace EphemereGames.Commander.Simulation
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
        public Path Path;
        public Path PathPreview;

        public event PhysicalObjectHandler ObjectDestroyed;

        private Simulator Simulator;

        private Action SyncUpdateShootingStars;
        private Action SyncPathPreview;

        private Particle Stars;
        private double StarsEmitter;

        private Core.Utilities.Pool<ShootingStar> ShootingStarsFactory;
        private bool DeadlyShootingStars;
        private bool FinalSolution;


        public PlanetarySystemController(Simulator simulator)
        {
            Simulator = simulator;
            ShootingStars = new List<ShootingStar>();
            ShootingStarsFactory = new Core.Utilities.Pool<ShootingStar>();

            Path = new Path(Simulator, new Color(255, 255, 255, 100), TypeBlend.Add);
            PathPreview = new Path(Simulator, new Color(255, 255, 255, 0), TypeBlend.Add);

            SyncUpdateShootingStars = new Action(UpdateShootingStars);
            SyncPathPreview = new Action(PathPreview.Update);
        }


        public void Initialize()
        {
            ShootingStars.Clear();

            Path.CelestialBodies = CelestialBodies;
            Path.Initialize();

            PathPreview.CelestialBodies = CelestialBodies;
            PathPreview.Initialize();

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

                    if (Path.ContainsCelestialBody(c))
                    {
                        Path.RemoveCelestialBody(c);
                        PathPreview.RemoveCelestialBody(c);
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
                celestialBody.PathPriority != -1 &&
                !Path.ContainsCelestialBody(celestialBody))
            {
                Path.AddCelestialBody(celestialBody);
                PathPreview.AddCelestialBody(celestialBody);
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
                Path.RemoveCelestialBody(celestialBody);
                PathPreview.RemoveCelestialBody(celestialBody);
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


        public List<CelestialBodyDescriptor> GenerateDescriptor()
        {
            List<CelestialBodyDescriptor> descriptor = new List<CelestialBodyDescriptor>();

            foreach (var c in CelestialBodies)
                descriptor.Add(c.GenerateDescriptor());

            return descriptor;
        }


        public void DoEditorCelestialBodyCommandExecuted(EditorPlayer player, EditorCelestialBodyCommand command)
        {
            if (command.Name == "Add")
            {
                var celestialBody = EditorLevelGenerator.GenerateCelestialBody(Simulator, CelestialBodies, Preferences.PrioriteSimulationCorpsCeleste);
                celestialBody.PathPriority = GetHighestPathPriority() + 1;
                CelestialBodies.Add(celestialBody);
            }

            else if (command.Name == "Remove")
            {
                command.CelestialBody.LifePoints = 0;
            }

            else if (command.Name == "ToggleSpeed")
            {
                command.CelestialBody.SetSpeed(command.Speed);
            }

            else if (command.Name == "PushFirst")
            {
                Path.RemoveCelestialBody(command.CelestialBody);
                command.CelestialBody.PathPriority = GetLowestPathPriority() - 1;
                Path.AddCelestialBody(command.CelestialBody);
            }


            else if (command.Name == "PushLast")
            {
                Path.RemoveCelestialBody(command.CelestialBody);
                command.CelestialBody.PathPriority = GetHighestPathPriority() + 1;
                Path.AddCelestialBody(command.CelestialBody);
            }


            else if (command.Name == "ToggleSize")
            {
                command.CelestialBody.SetSize(command.Size);
                command.CelestialBody.SetImage(command.CelestialBody.PartialImageName);
            }

            else if (command.Name == "AddGravitationalTurret")
            {
                command.CelestialBody.AddToStartingPath();
                Path.AddCelestialBody(command.CelestialBody);
                PathPreview.AddCelestialBody(command.CelestialBody);
            }

            else if (command.Name == "RemoveGravitationalTurret")
            {
                command.CelestialBody.RemoveFromStartingPath();
                Path.RemoveCelestialBody(command.CelestialBody);
                PathPreview.RemoveCelestialBody(command.CelestialBody);
            }

            else if (command.Name == "Clear")
            {
                foreach (var c in CelestialBodies)
                    c.LifePoints = 0;
            }
        }


        private int GetHighestPathPriority()
        {
            int highestPriorty = 0;

            foreach (var c in CelestialBodies)
                if (c.PathPriority > highestPriorty)
                    highestPriorty = c.PathPriority;

            return highestPriorty;
        }


        private int GetLowestPathPriority()
        {
            int lowestPriority = (CelestialBodies.Count == 0) ? 0 : CelestialBodies[0].PathPriority;

            foreach (var c in CelestialBodies)
                if (c.PathPriority < lowestPriority)
                    lowestPriority = c.PathPriority;

            return lowestPriority;
        }
    }
}
