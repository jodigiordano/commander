﻿namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Persistence;
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;
    using ProjectMercury.Emitters;


    class MineBullet : Bullet
    {
        public SimPlayer Owner;

        private Sprite ImageAlt;


        public MineBullet()
            : base()
        {
            Shape = Shape.Circle;
            Circle = new Circle(Vector3.Zero, 0);
            Rectangle = new PhysicalRectangle();
            Explosive = true;
            SfxExplosion = @"sfxMineExplose";
        }


        public override void LoadAssets()
        {
            ImageAlt = Persistence.GetAssetCopy<Sprite>("mine");
            ImageAlt.Taille = 2;
            ImageAlt.Origine = ImageAlt.Centre;
            Circle.Radius = 15;

            base.LoadAssets();
        }


        public override void Initialize()
        {
            base.Initialize();

            Update();

            ImageAlt.Position = Position;
            ImageAlt.VisualPriority = VisualPriority;

            ExplodingEffect = Scene.Particles.Get(@"projectileMissileExplosion");

            LifePoints = 100;
            AttackPoints = 100;
        }


        public override void Update()
        {
            ImageAlt.Update();

            base.Update();
        }


        public override void DoDie()
        {
            ((CircleEmitter) ExplodingEffect.Model[1]).Radius = ExplosionRange;
            
            base.DoDie();
        }


        public override void Draw()
        {
            if (Alive)
            {
                ImageAlt.Position = this.Position;
                Scene.Add(ImageAlt);
            }
        }
    }
}