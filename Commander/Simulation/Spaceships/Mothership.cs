namespace EphemereGames.Commander
{
    using System.Collections.Generic;
    using EphemereGames.Commander.Simulation;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class Mothership : IPhysical
    {
        public Image Base;
        public Image Top;
        public Image Lights;
        public Image Tentacles;

        private List<Missile> Missiles;



        public Mothership(double visualPriority)
        {
            Base = new Image("MothershipBase")
            {
                VisualPriority = visualPriority
            };

            Top = new Image("MothershipTop")
            {
                Color = new Color(255, 200, 0),
                Alpha = 255 / 2,
                VisualPriority = visualPriority - 0.000002
            };

            Lights = new Image("MothershipLights")
            {
                Color = new Color(255, 0, 0),
                Alpha = 0,
                VisualPriority = visualPriority - 0.000001
            
            };

            Tentacles = new Image("MothershipTentacles")
            {
                VisualPriority = visualPriority + 0.000001
            };

            SizeX = 16;

            Missiles = new List<Missile>();
        }


        public BlendType Blend
        {
            set
            {
                Base.Blend = value;
                Top.Blend = value;
                Lights.Blend = value;
                Tentacles.Blend = value;
            }
        }


        public float SizeX
        {
            set
            {
                Base.SizeX = value;
                Top.SizeX = value;
                Lights.SizeX = value;
                Tentacles.SizeX = value;
            }
        }


        public Vector2 Size
        {
            get { return Base.AbsoluteSize; }
        }


        public Vector3 Position
        {
            get { return Base.Position; }
            set { Base.Position = Top.Position = Lights.Position = Tentacles.Position = value; }
        }


        public float Speed
        {
            get { return 0; }
            set { }
        }


        public float Rotation
        {
            get { return Base.Rotation; }
            set { Base.Rotation = Lights.Rotation = Tentacles.Rotation = value; }
        }


        public void Update()
        {
            foreach (var m in Missiles)
                m.Update();
        }


        public void Draw(Scene scene)
        {
            scene.Add(Base);
            scene.Add(Top);
            scene.Add(Lights);
            scene.Add(Tentacles);

            foreach (var m in Missiles)
                m.Draw();
        }


        public void ActivateDeadlyLights(Scene scene, double time)
        {
            scene.VisualEffects.Add(Lights, Core.Visual.VisualEffects.Fade(Lights.Alpha, 255, 0, time));
            scene.VisualEffects.Add(Top, Core.Visual.VisualEffects.ChangeColor(Color.Red, 0, time));
        }


        public void DeactivateDeadlyLights(Scene scene, double time)
        {
            scene.VisualEffects.Add(Lights, Core.Visual.VisualEffects.Fade(Lights.Alpha, 0, 0, time));
            scene.VisualEffects.Add(Top, Core.Visual.VisualEffects.ChangeColor(new Color(255, 200, 0), 0, time));
        }


        public void DestroyEverything(Scene scene, List<CelestialBody> celestialBodies)
        {
            for (int i = 0; i < celestialBodies.Count; i++)
            {
                var cb = celestialBodies[i];

                Missiles.Add(new Missile(scene, cb, Position + new Vector3(0, Size.Y / 4, 0), i * 300));
            }
        }


        private class Missile
        {
            private Image Image;
            private Particle Effect;
            private CelestialBody CelestialBody;


            public Missile(Scene scene, CelestialBody celestialBody, Vector3 position, double delay)
            {
                CelestialBody = celestialBody;

                Image = new Image("PixelBlanc")
                {
                    Position = position,
                    VisualPriority = 1,
                    Alpha = 0,
                    SizeX = 50
                };

                Effect = scene.Particles.Get(@"mothershipMissile");
                Effect.VisualPriority = CelestialBody.VisualPriority - 0.00001;

                FollowEffect follow = new FollowEffect()
                {
                    Delay = delay,
                    Length = 10000,
                    FollowedObject = CelestialBody,
                    Speed = 2,
                    Progress = Core.Utilities.Effect<IPhysical>.ProgressType.Linear
                };

                scene.PhysicalEffects.Add(this.Image, follow);
            }


            public void Update()
            {
                if (!CelestialBody.Alive)
                    return;

                if (Vector3.DistanceSquared(Image.Position, CelestialBody.Position) <= 300)
                    CelestialBody.LifePoints = 0;
            }


            public void Draw()
            {
                if (!CelestialBody.Alive)
                    return;

                Effect.Trigger(ref Image.position);
            }
        }
    }
}
