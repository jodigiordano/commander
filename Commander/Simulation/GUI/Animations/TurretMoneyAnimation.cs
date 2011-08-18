namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class TurretMoneyAnimation : Animation
    {
        private Text MoneySpent;


        public TurretMoneyAnimation(int value, bool plusSign, Vector3 position, double visualPriority)
            : base(1000, visualPriority)
        {
            MoneySpent = new Text((plusSign? "+" : "-") + value + "$", "Pixelite", position)
            {
                SizeX = 2,
                VisualPriority = visualPriority
            }.CenterIt();
        }


        public override void Initialize()
        {
            base.Initialize();

            Scene.VisualEffects.Add(MoneySpent, Core.Visual.VisualEffects.FadeOutTo0(255, 0, 1000));
            Scene.PhysicalEffects.Add(MoneySpent, Core.Physics.PhysicalEffects.Move(MoneySpent.Position + new Vector3(0, -100, 0), 0, 1000));
        }


        public override void Draw()
        {
            Scene.Add(MoneySpent);
        }
    }
}
