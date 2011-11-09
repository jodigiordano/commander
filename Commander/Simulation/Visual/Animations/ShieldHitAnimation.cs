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


        public ShieldHitAnimation(string shieldName, ICollidable obj, Vector3 hitPosition, Color color, float sizeX, double visualPriority, float distFromSpaceship, byte alpha)
            : base(300, visualPriority)
        {
            Obj = obj;
            RelHitPosition = hitPosition - Obj.Position;

            Vector3 direction = hitPosition - Obj.Position;
            direction.Normalize();
            float rotation = Core.Physics.Utilities.VectorToAngle(direction);

            RelHitPosition -= direction * sizeX * 6;

            Shield = new Image(shieldName, Obj.Position + RelHitPosition)
            {
                SizeX = sizeX,
                Rotation = rotation,
                VisualPriority = visualPriority,
                Color = color,
                Alpha = alpha,
                Blend = BlendType.Add
            };
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
    }
}
