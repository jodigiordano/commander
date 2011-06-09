namespace EphemereGames.Commander
{
    using EphemereGames.Core.Physique;
    using EphemereGames.Core.Utilities;
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;


    class Bullet : IObjetPhysique, ILivingObject
    {
        protected Vector3 position;
        public Vector3 Position                                     { get { return position; } set { position = value; } }
        public float Speed                                          { get; set; }
        public float Masse                                          { get; set; }
        public float Rotation                                       { get; set; }
        public float ResistanceRotation                             { get; set; }
        public Vector3 Direction                                    { get; set; }
        public Shape Shape                                          { get; set; }
        public Cercle Circle                                        { get; set; }
        public Ligne Line                                           { get; set; }

        public RectanglePhysique Rectangle                          { get; set; }
        public float LifePoints                                     { get; set; }
        public float AttackPoints                                   { get; set; }

        public Scene Scene;
        public double PrioriteAffichage;
        public bool Alive                                           { get { return !OutOfBounds && LifePoints > 0; } }
        public bool OutOfBounds;
        public Image Image;
        public Particle ExplodingEffect;
        public Particle MovingEffect;
        public float DeflectZone;
        public BulletType Type;
        public bool AssetsLoaded;


        public Bullet()
        {
            Shape = Shape.Rectangle;
            DeflectZone = 100;
            AssetsLoaded = false;
        }


        public virtual void Initialize()
        {
            if (Direction != Vector3.Zero)
            {
                Vector3 direction = Direction;
                direction.Normalize();
                Direction = direction;
            }

            OutOfBounds = false;
        }


        public virtual void LoadAssets()
        {
            AssetsLoaded = true;
        }


        public virtual void Update()
        {
            switch (Shape)
            {
                case Shape.Rectangle:
                    Rectangle.X = (int)Position.X - Rectangle.Width / 2;
                    Rectangle.Y = (int)Position.Y - Rectangle.Height / 2;
                    break;

                case Shape.Circle:
                    Circle.Position = this.Position;
                    Rectangle.X = (int) (Position.X - Circle.Radius);
                    Rectangle.Y = (int) (Position.Y - Circle.Radius);
                    Rectangle.Width = (int) (Circle.Radius * 2);
                    Rectangle.Height = (int) (Circle.Radius * 2);
                    break;
            }
        }


        public virtual void Show()
        {
            if (Image != null)
                Scene.Add(Image);
        }


        public virtual void Hide()
        {
            if (Image != null)
                Scene.Remove(Image);
        }


        public virtual void Draw()
        {
            if (Alive && MovingEffect != null)
                MovingEffect.Trigger(ref this.position);

            if (Alive && Image != null)
                Image.Position = this.Position;
        }


        public virtual void DoHit(ILivingObject by)
        {
            LifePoints = 0;
        }


        public virtual void DoDie()
        {
            LifePoints = 0;

            if (ExplodingEffect != null)
                ExplodingEffect.Trigger(ref this.position);

            if (MovingEffect != null)
                Scene.Particles.Return(MovingEffect);

            if (ExplodingEffect != null)
                Scene.Particles.Return(ExplodingEffect);
        }


        public virtual void DoDieSilent()
        {
            LifePoints = 0;

            if (MovingEffect != null)
                Scene.Particles.Return(MovingEffect);

            if (ExplodingEffect != null)
                Scene.Particles.Return(ExplodingEffect);
        }
    }
}