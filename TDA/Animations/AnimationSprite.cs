namespace EphemereGames.Commander
{
    using EphemereGames.Core.Visuel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    
    class AnimationSprite : Animation
    {
        private Sprite Sprite;
        private double TimeSinceLastFrame = 0;


        public AnimationSprite(Sprite sprite)
            : base (sprite.TempsDefilement)
        {
            this.Sprite = sprite;
            this.Blend = sprite.Blend;
        }


        public override void Update(GameTime gameTime)
        {
            if (TimeSinceLastFrame >= Sprite.VitesseDefilement)
            {
                Sprite.suivant();
                TimeSinceLastFrame = 0;
            }
            
            TimeSinceLastFrame += gameTime.ElapsedGameTime.TotalMilliseconds;

            base.Update(gameTime);
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            Sprite.Draw(spriteBatch);
        }
    }
}
