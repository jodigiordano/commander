namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Modifiers;


    class Mothership : Spaceship
    {
        private Image Aliens;
        private Image Bubble;
        private Image Deadly;
        private NewSprite Eyes;
        private Image InvasionShips;
        private Image Lights;
        private Image Tentacles;

        private Particle AbductionEffect;
        private double AbductionLength;
        private bool Abducting;
        private double AbductionElapsed;
        private ICollidable ObjectAbducted;


        private List<Missile> Missiles;


        public Mothership(Simulator simulator, double visualPriority)
            : base(simulator)
        {
            Aliens = new Image(@"MothershipAliens")
            {
                VisualPriority = visualPriority + 0.000008
            };

            Image = new Image(@"MothershipBase")
            {
                VisualPriority = visualPriority + 0.000009
            };

            Bubble = new Image(@"MothershipBubble")
            {
                Blend = BlendType.Add,
                Alpha = 200,
                VisualPriority = visualPriority + 0.000005

            };

            Deadly = new Image(@"MothershipDeadly")
            {
                Alpha = 0,
                VisualPriority = visualPriority + 0.000004

            };

            Eyes = new NewSprite(@"MothershipEyesSprite", new List<int>()
            {
                0, 0, 0, 0, 0, 0, 0, 1,
                0, 0, 0, 0, 2, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 3,
                0, 0, 0, 0, 0, 0, 4, 0
            }, 200, true)
            {
                VisualPriority = visualPriority + 0.000005
            };

            InvasionShips = new Image(@"MothershipInvasionShips")
            {
                Alpha = 0,
                VisualPriority = visualPriority + 0.000006

            };

            Lights = new Image(@"MothershipLights")
            {
                VisualPriority = visualPriority + 0.000007
            
            };

            Tentacles = new Image(@"MothershipTentacles")
            {
                VisualPriority = visualPriority + 0.000010
            };

            SizeX = 16;
            Circle.Radius = Image.AbsoluteSize.X / 2;
            ShieldCircle.Radius = Circle.Radius + 10;

            Missiles = new List<Missile>();

            AbductionEffect = Simulator.Scene.Particles.Get(@"mothershipAbduction");
            ((RadialGravityModifier) AbductionEffect.Model[0].Modifiers[0]).Radius = 2000;
            ((RadialGravityModifier) AbductionEffect.Model[0].Modifiers[0]).Strength = 700;
            AbductionLength = 5000;
            AbductionElapsed = 0;
            Abducting = false;
        }


        public override BlendType Blend
        {
            set
            {
                base.Blend = value;

                Aliens.Blend = value;
                Bubble.Blend = value;
                Deadly.Blend = value;
                Eyes.Blend = value;
                InvasionShips.Blend = value;
                Lights.Blend = value;
                Tentacles.Blend = value;
            }
        }


        public override float SizeX
        {
            get
            {
                return base.SizeX;
            }

            set
            {
                base.SizeX = value;

                if (Aliens == null)
                    return;

                Aliens.SizeX = value;
                Bubble.SizeX = value;
                Deadly.SizeX = value;
                Eyes.SizeX = value;
                InvasionShips.SizeX = value;
                Lights.SizeX = value;
                Tentacles.SizeX = value;
            }
        }


        public Vector2 Size
        {
            get { return Image.AbsoluteSize; }
        }


        public override void Update()
        {
            base.Update();

            Eyes.Update();

            foreach (var m in Missiles)
                m.Update();

            if (Abducting)
            {
                AbductionElapsed += Preferences.TargetElapsedTimeMs;

                var p = ObjectAbducted.Position;

                ((RadialGravityModifier) AbductionEffect.Model[0].Modifiers[0]).Position = new Vector2(Position.X, Position.Y);
                AbductionEffect.Trigger(ref p);

                if (AbductionElapsed >= AbductionLength)
                {
                    Abducting = false;
                }
            }
        }


        public void Draw(Scene scene)
        {
            base.Draw();

            Aliens.Position =
            Bubble.Position =
            Deadly.Position =
            Eyes.Position =
            InvasionShips.Position =
            Lights.Position =
            Tentacles.Position = Image.Position;

            Aliens.Rotation =
            Bubble.Rotation =
            Deadly.Rotation =
            Eyes.Rotation =
            InvasionShips.Rotation =
            Lights.Rotation =
            Tentacles.Rotation = Image.Rotation;

            scene.Add(Aliens);
            scene.Add(Bubble);
            scene.Add(Deadly);
            scene.Add(Eyes);
            scene.Add(InvasionShips);
            scene.Add(Lights);
            scene.Add(Tentacles);

            foreach (var m in Missiles)
                m.Draw();
        }


        public void ActivateDeadlyLights(Scene scene, double time)
        {
            Eyes.Stop();
            Eyes.FirstFrame();
            scene.VisualEffects.Add(Deadly, Core.Visual.VisualEffects.Fade(Deadly.Alpha, 255, 0, time));
        }


        public void DeactivateDeadlyLights(Scene scene, double time)
        {
            scene.VisualEffects.Add(Deadly, Core.Visual.VisualEffects.Fade(Deadly.Alpha, 0, 0, time));
        }


        public void DestroyEverything(Scene scene, List<CelestialBody> celestialBodies)
        {
            for (int i = 0; i < celestialBodies.Count; i++)
            {
                var cb = celestialBodies[i];

                Missiles.Add(new Missile(scene, cb, cb.VisualPriority - 0.000001, Position + new Vector3(0, Size.Y / 4, 0), i * 300, DoTargetReached));
            }
        }


        public void DestroyEverything(Scene scene, List<HumanBattleship> battleships)
        {
            for (int i = 0; i < battleships.Count; i++)
            {
                var ship = battleships[i];

                Missiles.Add(new Missile(scene, ship, ship.VisualPriority - 0.000001, Position + new Vector3(0, Size.Y / 4, 0), i * 300, DoTargetReached));
            }
        }


        public void CoverInvasionShips(Scene scene, double time)
        {
            scene.VisualEffects.Add(InvasionShips, Core.Visual.VisualEffects.Fade(InvasionShips.Alpha, 255, 0, time));
        }


        public void AbductShip(Scene scene, ICollidable obj, double visualPriority)
        {
            Abducting = true;
            AbductionElapsed = 0;
            ObjectAbducted = obj;
            AbductionEffect.VisualPriority = visualPriority;
        }



        private void DoTargetReached(IDestroyable obj)
        {
            var cb = obj as CelestialBody;

            if (cb != null)
            {
                cb.LifePoints = 0;
                return;
            }

            var bs = obj as HumanBattleship;

            if (bs != null)
            {
                bs.DoDie();
                bs.Alpha = 0;
                return;
            }
        }


        private class Missile
        {
            private Image Image;
            private Particle Effect;
            private IDestroyable FollowedObject;
            private DestroyableHandler TargetReachedCallback;


            public Missile(Scene scene, IDestroyable followedObject, double visualPriority, Vector3 position, double delay, DestroyableHandler targetReachedCallback)
            {
                FollowedObject = followedObject;
                TargetReachedCallback = targetReachedCallback;

                Image = new Image("PixelBlanc")
                {
                    Position = position,
                    VisualPriority = 1,
                    Alpha = 0,
                    SizeX = 50
                };

                Effect = scene.Particles.Get(@"mothershipMissile");
                Effect.VisualPriority = visualPriority;

                FollowEffect follow = new FollowEffect()
                {
                    Delay = delay,
                    Length = 100000,
                    FollowedObject = FollowedObject,
                    Speed = 1.5f,
                    Progress = Core.Utilities.Effect<IPhysical>.ProgressType.Linear
                };

                scene.PhysicalEffects.Add(this.Image, follow);
            }


            public void Update()
            {
                if (!FollowedObject.Alive)
                    return;

                if (Vector3.DistanceSquared(Image.Position, FollowedObject.Position) <= 300)
                    TargetReachedCallback(FollowedObject);
            }


            public void Draw()
            {
                if (!FollowedObject.Alive)
                    return;

                Effect.Trigger(ref Image.position);
            }
        }
    }
}
