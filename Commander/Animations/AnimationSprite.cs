namespace EphemereGames.Commander
{
    using EphemereGames.Core.Visual;
    using Microsoft.Xna.Framework;


    class AnimationSprite : Animation
    {
        private Sprite Sprite;
        private double TimeSinceLastFrame = 0;


        public AnimationSprite(Sprite sprite)
            : base(sprite.TempsDefilement, sprite.VisualPriority)
        {
            this.Sprite = sprite;
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


        //protected override void Show()
        //{
        //    Scene.Add(Sprite);
        //}


        //protected override void Hide()
        //{
        //    Scene.Remove(Sprite);
        //}


        public override void Draw()
        {
            Scene.Add(Sprite);
        }
    }
}
