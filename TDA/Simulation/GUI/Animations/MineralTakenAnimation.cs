namespace EphemereGames.Commander
{
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;


    class MineralTakenAnimation : Animation
    {
        IVisible Value;


        public MineralTakenAnimation(Scene scene, MineralDefinition definition, Vector3 position)
            : base(1000)
        {
            Value = new IVisible(Core.Persistance.Facade.GetAsset<Texture2D>(definition.Texture), position);
            Value.Origine = Value.Centre;

            switch (definition.Type)
            {
                case MineralType.Cash150:
                    Value.Taille = 2;
                    break;
                case MineralType.Life1:
                    Value.Taille = 2;
                    break;
            }

            scene.Effets.Add(Value, Core.Visuel.PredefinedEffects.FadeOutTo0(255, 0, 1000));
            scene.Effets.Add(Value, Core.Physique.PredefinedEffects.Move(position + new Vector3(0, -100, 0), 0, 1000));
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            Value.Draw(spriteBatch);
        }
    }
}
