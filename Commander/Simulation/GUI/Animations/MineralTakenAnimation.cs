namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class MineralTakenAnimation : Animation
    {
        private Image Value;


        public MineralTakenAnimation(Scene scene, MineralDefinition definition, Vector3 position, double visualPriority)
            : base(1000, visualPriority)
        {
            Value = new Image(definition.Texture, position);

            switch (definition.Type)
            {
                case MineralType.Cash150:
                    Value.SizeX = 2;
                    break;
                case MineralType.Life1:
                    Value.SizeX = 2;
                    break;
            }

            scene.VisualEffects.Add(Value, Core.Visual.VisualEffects.FadeOutTo0(255, 0, 1000));
            scene.PhysicalEffects.Add(Value, Core.Physics.PhysicalEffects.Move(position + new Vector3(0, -100, 0), 0, 1000));
        }


        //protected override void Show()
        //{
        //    Scene.Add(Value);
        //}


        //protected override void Hide()
        //{
        //    Scene.Remove(Value);
        //}


        public override void Draw()
        {
            Scene.Add(Value);
        }
    }
}
