namespace EphemereGames.Commander
{
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;


    class MineralTakenAnimation : Animation
    {
        private Image Value;


        public MineralTakenAnimation(Scene scene, MineralDefinition definition, Vector3 position)
            : base(1000)
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

            scene.Effects.Add(Value, Core.Visuel.PredefinedEffects.FadeOutTo0(255, 0, 1000));
            scene.Effects.Add(Value, Core.Physique.PredefinedEffects.Move(position + new Vector3(0, -100, 0), 0, 1000));
        }


        protected override void Show()
        {
            Scene.Add(Value);
        }


        protected override void Hide()
        {
            Scene.Remove(Value);
        }
    }
}
