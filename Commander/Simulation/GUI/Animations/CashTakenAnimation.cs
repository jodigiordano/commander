namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class CashTakenAnimation : Animation
    {
        private Text Value;


        public CashTakenAnimation(int value, Vector3 position, double visualPriority)
            : base(1000, visualPriority)
        {
            Value = new Text(value + "M$", "Pixelite", position)
            {
                SizeX = 2,
                VisualPriority = visualPriority
            }.CenterIt();
        }


        public override void Initialize()
        {
            base.Initialize();

            Scene.VisualEffects.Add(Value, Core.Visual.VisualEffects.FadeOutTo0(255, 0, 1000));
            Scene.PhysicalEffects.Add(Value, Core.Physics.PhysicalEffects.Move(Value.Position + new Vector3(0, -100, 0), 0, 1000));
        }


        public override void Draw()
        {
            Scene.Add(Value);
        }
    }
}
