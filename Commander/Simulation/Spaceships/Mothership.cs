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

        private Simulator Simulator;
        private List<Missile> Missiles;



        public Mothership(Simulator simulator, double visualPriority)
        {
            Simulator = simulator;

            Base = new Image("MothershipBase")
            {
                SizeX = 16,
                VisualPriority = visualPriority
            };

            Top = new Image("MothershipTop")
            {
                Color = new Color(255, 200, 0),
                Alpha = 255 / 2,
                SizeX = 16,
                VisualPriority = visualPriority - 0.000002
            };

            Lights = new Image("MothershipLights")
            {
                Color = new Color(255, 0, 0),
                Alpha = 0,
                SizeX = 16,
                VisualPriority = visualPriority - 0.000001
            
            };

            Tentacles = new Image("MothershipTentacles")
            {
                SizeX = 16,
                VisualPriority = visualPriority + 0.000001
            };

            Missiles = new List<Missile>();
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


        public void Draw()
        {
            //Top.Rotation += 0.01f;

            //if (Top.Rotation > MathHelper.TwoPi)
            //    Top.Rotation -= MathHelper.TwoPi;

            Simulator.Scene.Add(Base);
            Simulator.Scene.Add(Top);
            Simulator.Scene.Add(Lights);
            Simulator.Scene.Add(Tentacles);

            foreach (var m in Missiles)
                m.Draw();
        }


        public void ActivateDeadlyLights(double time)
        {
            Simulator.Scene.VisualEffects.Add(Lights, Core.Visual.VisualEffects.Fade(Lights.Alpha, 255, 0, time));
            Simulator.Scene.VisualEffects.Add(Top, Core.Visual.VisualEffects.ChangeColor(Color.Red, 0, time));
        }


        public void DeactivateDeadlyLights(double time)
        {
            Simulator.Scene.VisualEffects.Add(Lights, Core.Visual.VisualEffects.Fade(Lights.Alpha, 0, 0, time));
            Simulator.Scene.VisualEffects.Add(Top, Core.Visual.VisualEffects.ChangeColor(new Color(255, 200, 0), 0, time));
        }


        public void DestroyEverything()
        {
            for (int i = 0; i < Simulator.PlanetarySystemController.CelestialBodies.Count; i++)
            {
                var cb = Simulator.PlanetarySystemController.CelestialBodies[i];

                Missiles.Add(new Missile(Simulator, cb, Position + new Vector3(0, Size.Y / 4, 0), i * 300));
            }
        }


        private class Missile
        {
            private Image Image;
            private Particle Effect;
            private CelestialBody CelestialBody;
            private Simulator Simulator;


            public Missile(Simulator simulator, CelestialBody celestialBody, Vector3 position, double delay)
            {
                Simulator = simulator;
                CelestialBody = celestialBody;

                Image = new Image("PixelBlanc")
                {
                    Position = position,
                    VisualPriority = 1,
                    Alpha = 0,
                    SizeX = 50
                };

                Effect = simulator.Scene.Particles.Get(@"mothershipMissile");
                Effect.VisualPriority = CelestialBody.VisualPriority - 0.00001;

                FollowEffect follow = new FollowEffect()
                {
                    Delay = delay,
                    Length = 10000,
                    FollowedObject = CelestialBody,
                    Speed = 2,
                    Progress = Core.Utilities.Effect<IPhysical>.ProgressType.Linear
                };

                simulator.Scene.PhysicalEffects.Add(this.Image, follow);
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
