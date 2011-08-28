namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class CelestialBodyShieldHitAnimation : Animation
    {
        private Image Shield;


        public CelestialBodyShieldHitAnimation(Size size, Vector3 position, float rotation, Color color, double visualPriority)
            : base(300, visualPriority)
        {
            Shield = new Image(size == Size.Small ? "CBMask31" : size == Size.Normal ? "CBMask32" : "CBMask33", position)
            {
                SizeX = 6,
                Rotation = rotation,
                VisualPriority = visualPriority,
                Color = color,
                Alpha = 200,
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
            Scene.Add(Shield);
        }
    }
}
