namespace EphemereGames.Commander
{
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;


    class RailGunBullet : Bullet
    {
        public float ZoneImpact;


        public RailGunBullet()
            : base()
        {
            Shape = Shape.Rectangle;
            Rectangle = new RectanglePhysique();
        }


        public override void Initialize()
        {
            base.Initialize();

            Rectangle.set((int) Position.X - 10, (int) Position.Y - 10, 20, 20);

            MovingEffect = Scene.Particles.Get("railgun");
            ExplodingEffect = Scene.Particles.Get("railgunExplosion");

            MovingEffect.VisualPriority = PrioriteAffichage + 0.001f;
            ExplodingEffect.VisualPriority = 0.35f;
            
            LifePoints = 5;
        }


        public override void Update()
        {
            Position += Speed * Direction;
            Rectangle.X = (int) Position.X;
            Rectangle.Y = (int) Position.Y;

            base.Update();
        }


        public override void DoDie()
        {
            EphemereGames.Core.Audio.Facade.jouerEffetSonore("Partie", "sfxRailGunExplosion2");
            
            base.DoDie();
        }
    }
}