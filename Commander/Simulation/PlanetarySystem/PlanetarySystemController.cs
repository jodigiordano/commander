namespace EphemereGames.Commander.Simulation
{
    using System;
    using System.Collections.Generic;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using ParallelTasks;
    using ProjectMercury.Emitters;


    class PlanetarySystemController
    {
        public event PhysicalObjectHandler ObjectHit;
        public event PhysicalObjectHandler ObjectDestroyed;

        private Simulator Simulator;

        private Action SyncUpdateShootingStars;
        private Action SyncPathPreview;

        private Particle Stars;
        private double StarsEmitter;

        private Core.Utilities.Pool<ShootingStar> ShootingStarsFactory;
        private bool DeadlyShootingStars;

        private List<CelestialBody> CelestialBodies { get { return Simulator.Data.Level.PlanetarySystem; } }
        private Path Path { get { return Simulator.Data.Path; } }
        private Path PathPreview { get { return Simulator.Data.PathPreview; } }
        private List<ShootingStar> ShootingStars { get { return Simulator.Data.ShootingStars; } }


        public PlanetarySystemController(Simulator simulator)
        {
            Simulator = simulator;
            ShootingStarsFactory = new Core.Utilities.Pool<ShootingStar>();

            SyncUpdateShootingStars = new Action(UpdateShootingStars);
            SyncPathPreview = new Action(PathPreview.Update);
        }


        public void Initialize()
        {
            if (Simulator.EditorEditingMode)
                foreach (var c in CelestialBodies)
                    c.CanSelectOverride = true;

            Stars = Simulator.Scene.Particles.Get(@"etoilesScintillantes");

            var emitter = (RectEmitter) Stars.Model[0];

            emitter.TriggerOffset = new Vector2(Simulator.Data.Battlefield.Outer.Center.X, Simulator.Data.Battlefield.Outer.Center.Y);
            emitter.Width = Simulator.Data.Battlefield.Outer.Width;
            emitter.Height = Simulator.Data.Battlefield.Outer.Height;
            emitter.ReleaseQuantity = (int) Math.Ceiling((Math.Max(Simulator.Data.Battlefield.Outer.Width, Simulator.Data.Battlefield.Outer.Height) / Math.Max(Preferences.BackBuffer.X, Preferences.BackBuffer.Y)));

            Stars.VisualPriority = VisualPriorities.Default.Stars;
            StarsEmitter = 0;

            DeadlyShootingStars = false;
        }


        public void Update()
        {
            for (int i = CelestialBodies.Count - 1; i > -1; i--)
            {
                CelestialBody c = CelestialBodies[i];

                if (!c.Alive)
                {
                    if (Simulator.CutsceneMode || Simulator.WorldMode)
                    {
                        c.SilentDeath = true;
                        c.SlowDeath = true;
                    }

                    c.DoDie();

                    if (!c.StayOnPathUponDeath && Path.ContainsCelestialBody(c))
                    {
                        Path.RemoveCelestialBody(c);
                        PathPreview.RemoveCelestialBody(c);
                    }

                    CelestialBodies.RemoveAt(i);

                    NotifyObjectDestroyed(c);
                }
            }

            UpdateCelestialBodies();


            Task t1, t2;

            t1 = Parallel.Start(SyncUpdateShootingStars);
            t2 = Parallel.Start(SyncPathPreview);

            Path.Update();

            t1.Wait();
            t2.Wait();

            StarsEmitter += Preferences.TargetElapsedTimeMs;

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
            {
                if (c.FirstOnPath && !c.LastOnPath && c.Image != null)
                {
                    c.Image.Rotation = Path.GetRotation(0) + MathHelper.PiOver2;
                }

                c.Draw();
            }
        }


        public bool AddTurret(Turret turret)
        {
            CelestialBody celestialBody = turret.CelestialBody;

            celestialBody.Turrets.Add(turret);

            if (turret.Type == TurretType.Gravitational &&
                celestialBody.PathPriority != int.MinValue &&
                !Path.ContainsCelestialBody(celestialBody))
            {
                Path.AddCelestialBody(celestialBody, true);
                PathPreview.AddCelestialBody(celestialBody, true);
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


        public void DoEnemyReachedEnd(Enemy enemy, CelestialBody celestialBody)
        {
            if (Simulator.State == GameState.Won)
                return;

            if (celestialBody == null || !celestialBody.Alive)
                return;

            if (!(celestialBody is AsteroidBelt))
                celestialBody.DoHit(enemy);

            if (!Simulator.DemoMode && celestialBody.Alive)
                NotifyObjectHit(celestialBody);
        }


        public void DoPowerUpStarted(PowerUp powerUp, SimPlayer player)
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

                var stars = Stars.Model[0];

                stars.ReleaseColour = newColor;

                for (int i = 0; i < stars.ActiveParticlesCount; i++)
                {
                    var star = stars.Particles[i];

                    star.Colour = newColor2;

                    var effect = Simulator.Scene.Particles.Get(@"starExplosion");
                    effect.Model[0].ReleaseColour = newColor;
                    effect.Trigger(ref star.Position);
                    effect.VisualPriority = Preferences.PrioriteFondEcran - 0.00001f;
                    Simulator.Scene.Particles.Return(effect);
                }
            }
        }


        public void DoPowerUpStopped(PowerUp powerUp, SimPlayer player)
        {
            if (powerUp.Type != PowerUpType.FinalSolution)
                return;

            PowerUpLastSolution p = (PowerUpLastSolution) powerUp;

            if (!p.GoAhead)
                return;

            p.CelestialBody.ZoneImpactDestruction = p.ZoneImpactDestruction;
            p.CelestialBody.AttackPoints = p.AttackPoints;
            p.CelestialBody.LifePoints = 0;
        }


        public List<CelestialBodyDescriptor> GenerateDescriptor()
        {
            List<CelestialBodyDescriptor> descriptor = new List<CelestialBodyDescriptor>();

            foreach (var c in CelestialBodies)
                descriptor.Add(c.GenerateDescriptor());

            return descriptor;
        }


        public void DoEditorCommandExecuted(EditorCommand c)
        {
            var command = c as EditorCelestialBodyCommand;

            if (command == null)
                return;

            if (command.Name == "AddPlanet" || command.Name == "AddPinkHole")
            {
                command.CelestialBody.PathPriority = GetLowestPathPriority(CelestialBodies) - 1;
                command.CelestialBody.AliveOverride = true;
                command.CelestialBody.CanSelectOverride = true;
                command.CelestialBody.Position = command.Owner.Position;
                command.CelestialBody.BasePosition = command.Owner.Position;
                CelestialBodies.Add(command.CelestialBody);
            }

            else if (command.Name == "Remove")
            {
                command.CelestialBody.LifePoints = 0;
                command.CelestialBody.AliveOverride = false;
            }

            else if (command.Name == "ToggleSpeed")
            {
                command.CelestialBody.SetSpeed(command.Speed);
            }

            else if (command.Name == "ChangeAsset")
            {
                command.CelestialBody.SetImage(command.AssetName);
            }

            else if (command.Name == "PushFirst")
            {
                if (!Path.ContainsCelestialBody(command.CelestialBody))
                    AddToStartingPath(command.CelestialBody);

                Path.RemoveCelestialBody(command.CelestialBody);
                command.CelestialBody.PathPriority = GetLowestPathPriority(CelestialBodies) - 1;
                Path.AddCelestialBody(command.CelestialBody, false);
            }


            else if (command.Name == "PushLast")
            {
                if (!Path.ContainsCelestialBody(command.CelestialBody))
                    AddToStartingPath(command.CelestialBody);

                Path.RemoveCelestialBody(command.CelestialBody);
                command.CelestialBody.PathPriority = GetHighestPathPriority(CelestialBodies) + 1;
                Path.AddCelestialBody(command.CelestialBody, false);
            }


            else if (command.Name == "ToggleSize")
            {
                command.CelestialBody.SetSize(command.Size);
                command.CelestialBody.SetImage(command.CelestialBody.PartialImageName);
            }


            else if (command.Name == "HasMoons")
            {
                command.CelestialBody.SetHasMoons(command.HasMoons);
            }


            else if (command.Name == "FollowPath")
            {
                command.CelestialBody.FollowPath = command.FollowPath;
            }


            else if (command.Name == "CanSelect")
            {
                command.CelestialBody.CanSelect = command.CanSelect;
            }


            else if (command.Name == "StraightLine")
            {
                command.CelestialBody.StraightLine = command.StraightLine;
            }


            else if (command.Name == "Invincible")
            {
                command.CelestialBody.Invincible = command.Invincible;
            }


            else if (command.Name == "RemoveFromPath")
            {
                RemoveFromStartingPath(command.CelestialBody);
            }

            //else if (command.Name == "Clear")
            //{
            //    foreach (var cb in CelestialBodies)
            //    {
            //        if (cb is AsteroidBelt)
            //            continue;

            //        cb.LifePoints = 0;
            //        cb.AliveOverride = false;
            //    }
            //}
        }


        public static int GetHighestPathPriority(List<CelestialBody> celestialBodies)
        {
            var c = GetCelestialBodyWithHighestPathPriority(celestialBodies);

            return c == null ? 0 : c.PathPriority;
        }


        public static int GetLowestPathPriority(List<CelestialBody> celestialBodies)
        {
            var c = GetCelestialBodyWithLowestPathPriority(celestialBodies);

            return c == null ? 0 : c.PathPriority;
        }


        public static CelestialBody GetAliveCelestialBodyWithHighestPathPriority(List<CelestialBody> celestialBodies)
        {
            if (celestialBodies.Count == 0)
                return null;

            CelestialBody highest = null;

            foreach (var c in celestialBodies)
                if (!(c is AsteroidBelt) && c.Alive && (highest == null || c.PathPriority > highest.PathPriority))
                    highest = c;

            return highest;
        }


        public static CelestialBody GetCelestialBodyWithHighestPathPriority(List<CelestialBody> celestialBodies)
        {
            if (celestialBodies.Count == 1)
                return null;

            CelestialBody highest = null;

            foreach (var c in celestialBodies)
                if (!(c is AsteroidBelt) && (highest == null || c.PathPriority > highest.PathPriority))
                    highest = c;

            return highest;
        }


        public static CelestialBody GetCelestialBodyWithLowestPathPriority(List<CelestialBody> celestialBodies)
        {
            if (celestialBodies.Count == 1)
                return null;

            CelestialBody lowest = null;

            foreach (var c in celestialBodies)
                if (!(c is AsteroidBelt) && (lowest == null || c.PathPriority < lowest.PathPriority))
                    lowest = c;

            return lowest;
        }


        public static AsteroidBelt GetAsteroidBelt(List<CelestialBody> CelestialBodies)
        {
            foreach (var c in CelestialBodies)
                if (c is AsteroidBelt)
                    return (AsteroidBelt) c;

            return null;
        }


        private void NotifyObjectHit(ICollidable obj)
        {
            if (ObjectHit != null)
                ObjectHit(obj);
        }


        private void NotifyObjectDestroyed(ICollidable obj)
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
                ss.Battlefield = Simulator.Data.Battlefield.Outer;
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


        private void RemoveFromStartingPath(CelestialBody celestialBody)
        {
            celestialBody.RemoveFromStartingPath();
            Path.RemoveCelestialBody(celestialBody);
            PathPreview.RemoveCelestialBody(celestialBody);
        }


        private void AddToStartingPath(CelestialBody celestialBody)
        {
            celestialBody.AddToStartingPath(false);

            if (celestialBody.PathPriority == int.MinValue)
                celestialBody.PathPriority = GetLowestPathPriority(CelestialBodies) - 1;

            Path.AddCelestialBody(celestialBody, false);
            PathPreview.AddCelestialBody(celestialBody, false);
        }


        private void UpdateCelestialBodies()
        {
            for (int i = 0; i < CelestialBodies.Count; i++)
            {
                var c = CelestialBodies[i];

                c.Update();
            }
        }
    }
}
