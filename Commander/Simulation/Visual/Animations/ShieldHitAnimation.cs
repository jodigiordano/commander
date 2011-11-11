namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Physics;
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class ShieldHitAnimation : Animation
    {
        private Image Shield;
        private ICollidable Obj;
        private Vector3 RelHitPosition;


        public ShieldHitAnimation(string shieldMaskName, Vector3 position, Vector3 hitPosition, Color color, float sizeX, double visualPriority, float shieldDistance, byte alpha)
            : this(shieldMaskName, new NoCollidable() { Position = position }, hitPosition, color, sizeX, visualPriority, shieldDistance, alpha)
        {

        }


        public ShieldHitAnimation(string shieldMaskName, ICollidable obj, Vector3 hitPosition, Color color, float sizeX, double visualPriority, float shieldDistance, byte alpha)
            : base(300, visualPriority)
        {
            Obj = obj;
            RelHitPosition = hitPosition - Obj.Position;

            Vector3 direction = hitPosition - Obj.Position;

            if (direction != Vector3.Zero)
                direction.Normalize();
            
            float rotation = Core.Physics.Utilities.VectorToAngle(direction);

            RelHitPosition -= direction * sizeX * 6;

            Shield = new Image(shieldMaskName, Obj.Position + RelHitPosition)
            {
                SizeX = sizeX,
                Rotation = rotation,
                VisualPriority = visualPriority,
                Color = color,
                Alpha = alpha,
                Blend = BlendType.Add
            };
        }


        public float Rotation
        {
            set { Shield.Rotation = value; }
        }


        public override void Initialize()
        {
            base.Initialize();

            Scene.VisualEffects.Add(Shield, Core.Visual.VisualEffects.FadeOutTo0(Shield.Alpha, 0, Duration));
        }


        public override void Draw()
        {
            Shield.Position = Obj.Position + RelHitPosition;
            Scene.Add(Shield);
        }


        public static bool IsHitFarEnough(Vector3 objPosition, Vector3 hitPosition, float minDistance)
        {
            Vector3 distance = hitPosition - objPosition;
            float distanceSquared = distance.LengthSquared();
            float minDistanceSquared = minDistance * minDistance;

            return distanceSquared >= minDistanceSquared;
        }


        private class NoCollidable : ICollidable
        {
            public Vector3 Position { get; set; }


            Vector3 ICollidable.Direction
            {
                get { throw new System.NotImplementedException(); }
                set { throw new System.NotImplementedException(); }
            }


            Shape ICollidable.Shape
            {
                get { throw new System.NotImplementedException(); }
                set { throw new System.NotImplementedException(); }
            }


            Circle ICollidable.Circle
            {
                get { throw new System.NotImplementedException(); }
                set { throw new System.NotImplementedException(); }
            }


            PhysicalRectangle ICollidable.Rectangle
            {
                get { throw new System.NotImplementedException(); }
                set { throw new System.NotImplementedException(); }
            }


            Line ICollidable.Line
            {
                get { throw new System.NotImplementedException(); }
                set { throw new System.NotImplementedException(); }
            }


            float IPhysical.Speed
            {
                get { throw new System.NotImplementedException(); }
                set { throw new System.NotImplementedException(); }
            }


            float IPhysical.Rotation
            {
                get { throw new System.NotImplementedException(); }
                set { throw new System.NotImplementedException(); }
            }
        }
    }
}
