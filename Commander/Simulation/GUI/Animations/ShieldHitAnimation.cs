namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class ShieldHitAnimation : Animation
    {
        private Image Shield;


        public ShieldHitAnimation(string shieldName, Vector3 objPosition, Vector3 hitPosition, Color color, float sizeX, double visualPriority, float distFromSpaceship, byte alpha)
            : base(300, visualPriority)
        {
            Vector3 direction = hitPosition - objPosition;
            direction.Normalize();
            float rotation = Core.Physics.Utilities.VectorToAngle(direction);

            Vector3 position = hitPosition - direction * sizeX * 6;

            Shield = new Image(shieldName, position)
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

            Scene.VisualEffects.Add(Shield, Core.Visual.VisualEffects.FadeOutTo0(Shield.Alpha, 0, Length));
        }


        public override void Draw()
        {
            Scene.Add(Shield);
        }
    }
}
