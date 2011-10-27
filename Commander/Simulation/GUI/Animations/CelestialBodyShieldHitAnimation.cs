namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class CelestialBodyShieldHitAnimation : Animation
    {
        private Image Shield;


        public CelestialBodyShieldHitAnimation(string maskName, Vector3 position, float rotation, Color color, double visualPriority, byte alpha)
            : base(300, visualPriority)
        {
            Shield = new Image(maskName, position)
            {
                SizeX = 6,
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
            Scene.Add(Shield);
        }
    }
}
